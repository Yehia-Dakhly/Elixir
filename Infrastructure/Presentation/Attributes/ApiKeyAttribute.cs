using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Attributes
{
    internal class ApiKeyAttribute : ActionFilterAttribute
    {
        private const string APIKEYNAME = "x-api-key";
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var hasSkipAttribute = context.ActionDescriptor.EndpointMetadata
                                          .Any(em => em.GetType() == typeof(SkipApiKeyAttribute));

            if (hasSkipAttribute)
            {
                await next();
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key is missing!");
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetValue<string>("ApiSettings:ApiKey");
            Console.WriteLine(apiKey);
            if (!apiKey!.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Invalid API Key!");
                return;
            }

            await next();
        }
    }
}
