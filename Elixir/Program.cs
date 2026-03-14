using Blood_Donation.Hubs;
using Blood_Donation.MiddleWares;
using Blood_Donation.Services;
using Blood_Donation.Workers;
using DomainLayer;
using DomainLayer.Contracts;
using DomainLayer.Models;
using DomainLayer.Optopns;
using Google;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Data;
using Persistence.Repositories;
using Serilog;
using Serilog.Events;
using Service;
using Service.Consumers;
using ServiceAbstraction;
using Shared.ErrorModels;
using Shared.Events;
using StackExchange.Redis;
using System.Text;


namespace Blood_Donation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                path: "Logs/elixir-log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Seq("http://localhost:5341") // Add Seq sink
                .WriteTo.NewRelicLogs(
                    endpointUrl: "https://log-api.eu.newrelic.com/log/v1",
                    applicationName: builder.Configuration["NewRelic:AppName"],
                    licenseKey: builder.Configuration["NewRelic:LicenseKey"]
                )
                .CreateLogger();

            try
            {
                Log.Information("Starting Elixir API Application...");

                builder.Services.AddControllers();
                //builder.Services.AddCors(Options =>
                //{
                //    Options.AddPolicy("AllowAll", Builder =>
                //    {
                //        Builder.AllowAnyHeader();
                //        Builder.AllowAnyMethod();
                //        Builder.AllowAnyOrigin();
                //    });
                //});
                builder.Host.UseSerilog(); // Tell the host to use Serilog for logging

                #region API Key
                //var key = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
                //Console.WriteLine(key); 
                #endregion

                #region AddSwaggerService
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(Options =>
                {
                    Options.CustomSchemaIds(type => type.ToString());
                    Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        Description = "Enter 'Bearer' Followed By Space And Your Token"
                    });

                    Options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme,
                            }
                        },
                        new string[] { }
                    }
                    });
                });
                #endregion

                builder.Services.AddHttpContextAccessor();

                #region DbContext - DB
                builder.Services.AddDbContext<BloodDonationDbContext>(Options =>
                {
                    Options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                });
                #endregion

                #region Services
                builder.Services.AddScoped<IDataSeed, DataSeed>();
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
                builder.Services.AddScoped<ICacheRepository, CacheRepository>();
                builder.Services.AddAutoMapper(cnf => { }, typeof(Service.AssemplyReference).Assembly);


                builder.Services.AddScoped<ILookupService, LookupService>();
                builder.Services.AddScoped<Func<ILookupService>>(
                    Provider => () =>
                    Provider.GetRequiredService<ILookupService>());
                builder.Services.AddScoped<IRequestService, RequestService>();
                builder.Services.AddScoped<Func<IRequestService>>
                    (
                        Provider =>
                        () => Provider.GetRequiredService<IRequestService>()
                    );
                builder.Services.AddScoped<IResponseService, ResponseService>();
                builder.Services.AddScoped<Func<IResponseService>>
                    (
                        Provider =>
                        () => Provider.GetRequiredService<IResponseService>()
                    );
                builder.Services.AddScoped<ICompatibilityService, CompatibilityService>();
                builder.Services.AddScoped<Func<ICompatibilityService>>
                    (
                        Provider =>
                        () => Provider.GetRequiredService<ICompatibilityService>()
                    );

                builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
                builder.Services.AddScoped<Func<IAuthenticationService>>
                    (
                        Provider =>
                        () => Provider.GetRequiredService<IAuthenticationService>()
                    );

                builder.Services.AddScoped<INotificationService, NotificationService>();
                builder.Services.AddScoped<Func<INotificationService>>
                    (
                        Provider =>
                        () => Provider.GetRequiredService<INotificationService>()
                    );

                builder.Services.AddScoped<IDashboardService, DashboardService>();
                builder.Services.AddScoped<Func<IDashboardService>>
                    (
                        Provider =>
                        () => Provider.GetRequiredService<IDashboardService>()
                    );


                builder.Services.AddScoped<IServiceManager, ServiceManager>();
                builder.Services.AddScoped<ICacheService, CacheService>();
                builder.Services.AddScoped<IGeoLocationService, GeoLocationService>();
                builder.Services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();
                builder.Services.AddScoped<IAccountService, AccountService>();

                builder.Services.AddSignalR();
                builder.Services.AddScoped<IRequestsUpdate, RequestsUpdateService>();
                #endregion

                #region Validation Errors Handling
                builder.Services.Configure<ApiBehaviorOptions>((options) =>
                    {
                        options.InvalidModelStateResponseFactory = (Context) =>
                        {
                            var Errors = Context.ModelState.Where(M => M.Value.Errors.Any())
                            .Select(M => new ValidationError()
                            {
                                Field = M.Key,
                                Errors = M.Value.Errors.Select(E => E.ErrorMessage)
                            });
                            var Response = new ValidationErrorToReturn()
                            {
                                Errors = Errors,
                                Message = $"فشل التحقق من البيانات.\nمعرف الإرتباط: {Context.HttpContext.Items["CorrelationId"]}"
                            };
                            return new BadRequestObjectResult(Response);
                        };
                    });
                #endregion

                #region Redis
                builder.Services.AddSingleton<IConnectionMultiplexer>((_) =>
                {
                    return ConnectionMultiplexer.Connect(builder.Configuration["ConnectionStrings:RedisConnection"]!);
                });
                #endregion

                #region RabbitMQ
                builder.Services.AddMassTransit(conf =>
                {
                    conf.SetKebabCaseEndpointNameFormatter();
                    conf.AddConsumer<NotifyDonorsWithNewRequest>();
                    conf.AddConsumer<RequestResponsed>();
                    conf.AddConsumer<UpdateLocationAndSendEmailForRegisteration>();
                    conf.AddConsumer<SendEmail>();
                    conf.AddConsumer<SendNotification>();
                    conf.AddConsumer<SystemNotificationFired>();
                    conf.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(builder.Configuration["RabbitMQ:Host"], builder.Configuration["RabbitMQ:VirtualHost"], H =>
                        {
                            H.Username(builder.Configuration["RabbitMQ:Username"]);
                            H.Password(builder.Configuration["RabbitMQ:Password"]);
                        });
                        cfg.ReceiveEndpoint("blood-request-created", e =>
                        {
                            e.ConfigureConsumer<NotifyDonorsWithNewRequest>(context);
                        });
                        cfg.ReceiveEndpoint("blood-request-responsed", e =>
                        {
                            e.ConfigureConsumer<RequestResponsed>(context);
                        });
                        cfg.ReceiveEndpoint("new-user-registered", e =>
                        {
                            e.ConfigureConsumer<UpdateLocationAndSendEmailForRegisteration>(context);
                        });
                        cfg.ReceiveEndpoint("send-emaill", e =>
                        {
                            e.ConfigureConsumer<SendEmail>(context);
                        });
                        cfg.ReceiveEndpoint("send-notification", e =>
                        {
                            e.ConfigureConsumer<SendNotification>(context);
                        });
                        cfg.ReceiveEndpoint("system-notification-fired", e =>
                        {
                            e.ConfigureConsumer<SystemNotificationFired>(context);
                        });
                        cfg.ConfigureEndpoints(context);
                    });
                });
                #endregion

                #region Auth
                // Default Providers
                builder.Services.Configure<DataProtectionTokenProviderOptions>(O => // Data Protector Token Provider
                {
                    O.TokenLifespan = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("BloodDonationSettings:TokenLifespanInMinutes"));
                });
                builder.Services.AddIdentityCore<BloodDonationUser>(Options =>
                    {
                        Options.User.RequireUniqueEmail = true;
                        Options.SignIn.RequireConfirmedEmail = true;
                    })
                        .AddRoles<IdentityRole<Guid>>()
                        .AddEntityFrameworkStores<BloodDonationDbContext>()
                        .AddDefaultTokenProviders()
                        .AddErrorDescriber<ArabicIdentityErrorDiscriber>();
                builder.Services.AddAuthentication(Options =>
                {
                    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(Options =>
                    {
                        Options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:Key"]!)),

                            ValidateIssuer = true,
                            ValidIssuer = builder.Configuration["JWTSettings:Issuer"],

                            ValidateAudience = true,
                            ValidAudience = builder.Configuration["JWTSettings:Audience"],

                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                    });
                #endregion

                #region System Health
                var healthChecksBuilder = builder.Services.AddHealthChecks()
                    .AddDbContextCheck<BloodDonationDbContext>(name: "SQL Database")
                    .AddRedis(builder.Configuration["ConnectionStrings:RedisConnection"]!, name: "Redis Cache")
                    .AddRabbitMQ(rabbitConnectionString: builder.Configuration["RabbitMQ:ConnectionString"]!, name: "Message Broker");

                builder.Services.AddHealthChecksUI(setup =>
                {
                    setup.SetEvaluationTimeInSeconds(600);
                    setup.MaximumHistoryEntriesPerEndpoint(60);
                    setup.AddHealthCheckEndpoint("Elixir API System", "/api/health");
                })
                .AddInMemoryStorage();
                #endregion


                // For Options Patterns
                builder.Services.Configure<BloodDonationSettings>(builder.Configuration.GetSection("BloodDonationSettings"));
                // For Worker
                builder.Services.AddHostedService<RedisDataSeeder>();
                var app = builder.Build();

                #region Data Seeding Roles & Data
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var _dbContext = services.GetRequiredService<BloodDonationDbContext>();
                        var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                        if (pendingMigrations.Any())
                        {
                            await _dbContext.Database.MigrateAsync();
                        }
                        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                        await RoleDataSeedser.SeedAsync(roleManager);
                        var ObjectOfDataSeeding = services.GetRequiredService<IDataSeed>();
                        await ObjectOfDataSeeding.DataSeedAsync();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while seeding roles.");
                    }
                }           
                #endregion

                app.UseMiddleware<CorrelationalIdMiddleWare>();
                app.UseSerilogRequestLogging();
                app.UseMiddleware<ExceptionMiddleWare>();
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseAuthorization();

                #region Health System Endpoints
                app.MapHealthChecks("/api/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse // UI.Client
                }).WithTags("System Health");
                // Dashboard
                app.MapHealthChecksUI(options =>
                {
                    options.UIPath = "/dashboard";
                });

                app.MapGet("/api/health/simple", async (HealthCheckService healthCheckService) =>
                {
                    var report = await healthCheckService.CheckHealthAsync();
                    bool isSystemUp = report.Status == HealthStatus.Healthy || report.Status == HealthStatus.Degraded;
                    return Results.Ok(isSystemUp);
                })
                .WithTags("System Health");
                app.MapGet("/api/health/details", async (HealthCheckService healthCheckService) =>
                {
                    var report = await healthCheckService.CheckHealthAsync();
                    return report;

                }).WithTags("System Health");
                #endregion

                app.MapControllers();
                app.MapHub<RequestsHub>("/requestsHub");

                app.MapGet("/", async context =>
                {
                    context.Response.Redirect("/swagger/index.html");
                    await Task.CompletedTask;
                });

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
