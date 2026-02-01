using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Exceptions.NotFoundExceptions;
using DomainLayer.Models;
using DomainLayer.Optopns;
using Google.Apis.Auth;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Specifications;
using ServiceAbstraction;
using Shared.DataTransferObjects.Authentication;
using Shared.DataTransferObjects.Authentication.PasswordsAndOTPDTos;
using Shared.DataTransferObjects.Authentication.ResetForgetChangePasswordDTos;
using Shared.Events;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static MassTransit.ValidationResultExtensions;
using static System.Net.WebRequestMethods;
using Gender = DomainLayer.Models.Gender;


namespace Service
{
    public class AuthenticationService(
        UserManager<BloodDonationUser> _userManager,
        IConfiguration _configuration,
        IOptionsSnapshot<BloodDonationSettings> _optionsSnapshot,
        IHttpContextAccessor httpContextAccessor,
        IPublishEndpoint _publishEndpoint,
        LinkGenerator _linkGenerator,
        IUnitOfWork _unitOfWork,
        IGeoLocationService _geoLocationService) : IAuthenticationService
    {
        private readonly BloodDonationSettings _settings = _optionsSnapshot.Value;
        public async Task<AuthUserDTo> RegisterAsync(RegisterDTo registerDTo)
        {
            Gender Gender = registerDTo.Gender == 0 ? Gender.Undefined : registerDTo.Gender == 1 ? Gender.Male : Gender.Female;
            var NewUser = new BloodDonationUser()
                {
                    FullName = registerDTo.FullName,
                    Email = registerDTo.Email,
                    Age = registerDTo.Age,
                    BloodTypeId = registerDTo.BloodTypeId,
                    Gender = Gender,
                    Latitude = registerDTo.Latitude,
                    Longitude = registerDTo.Longitude,
                    CityId = registerDTo.CityId,
                    UserName = registerDTo.Email,
                    PhoneNumber = registerDTo.PhoneNumber,
                    DeviceToken = registerDTo.DeviceToken,
                    LastTokenUpdate = DateTime.UtcNow,
                    IsAvailable = true,
                };
            var Result = await _userManager.CreateAsync(NewUser, registerDTo.Password);
            if (Result.Succeeded)
            {
                var contxt = httpContextAccessor.HttpContext ?? throw new UnauthorizedException("يرجى تسجيل الدخول أولاً");
                var Token = await _userManager.GenerateEmailConfirmationTokenAsync(NewUser);
                var EncodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(Token));

                var ConfirmationLink = _linkGenerator.GetUriByRouteValues
                    (
                        contxt,
                        routeName: "confirm-email",
                        values: new
                        {
                            Email = NewUser.Email,
                            Token = EncodedToken
                        },
                        scheme: contxt.Request.Scheme,
                        host: contxt.Request.Host
                    );
                await _publishEndpoint.Publish(new UserRegisteredEvent()
                {
                    ConfirmationLink = ConfirmationLink!,
                    UserId = NewUser.Id.ToString(),
                });
                var CityRepo = _unitOfWork.GetRepository<City, int>();
                var SpecificationCity = new CityWithGovernorateByCityId(NewUser.CityId);
                var City = await CityRepo.GetByIdAsync(SpecificationCity);
                return new AuthUserDTo()
                {
                    Email = registerDTo.Email,
                    FullName = registerDTo.FullName,
                    Token = await CreateTokenAsync(NewUser),
                    CityName = City.NameAr,
                    GovernorateName = City.Governorate.NameAr
                };
            }
            else
            {
                var Errors = Result.Errors.Select(E => E.Description).ToList();
                throw new BadRequestException(Errors);
            }
        }
        public async Task<AuthUserDTo> LoginAsync(LoginDTo loginDTo)
        {
            var User = await _userManager.Users.Include(U => U.City).ThenInclude(C => C.Governorate).FirstOrDefaultAsync(U => U.Email == loginDTo.Email) ?? throw new UserNotFoundException(loginDTo.Email);
            if (await _userManager.IsLockedOutAsync(User))
            {
                throw new UnauthorizedException("هذا الحساب محظور، يرجى التواصل مع الإدارة!");
            }
            if(await _userManager.IsEmailConfirmedAsync(User) == false)
            {
                throw new ForbiddenException("من فضلك فعّل بريدك الإلكتروني قبل تسجيل الدخول.");
            }
            if (await _userManager.CheckPasswordAsync(User, loginDTo.Password))
            {
                await _userManager.ResetAccessFailedCountAsync(User);
                if (loginDTo.DeviceToken is not null) // Consume
                {
                    User.DeviceToken = loginDTo.DeviceToken;
                    await _userManager.UpdateAsync(User);
                    await _geoLocationService.UpdateDonorLocationAndDeviceTokenAsync(User.Id.ToString(), User.DeviceToken, User.Longitude, User.Latitude, User.BloodTypeId.ToString());
                }
                return new AuthUserDTo() 
                { 
                    Email = loginDTo.Email,
                    FullName = User.FullName,
                    Token = await CreateTokenAsync(User),
                    CityName = User.City.NameAr,
                    GovernorateName = User.City.Governorate.NameAr,
                };
            }
            else
            {
                await _userManager.AccessFailedAsync(User);
                throw new UnauthorizedException("كلمة السر غير صحيحة!");
            }
        }
        public async Task<AuthUserDTo> GoogleLoginAsync(GoogleLoginDTo googleLoginDTo)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["GoogleAuthentication:ClientId"]! }
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDTo.IdToken, settings);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Invalid Google Token");
            }
            var User = await _userManager.Users.Include(U => U.City).ThenInclude(C => C.Governorate).FirstOrDefaultAsync(U => U.Email == payload.Email) ?? throw new UserNotFoundException(payload.Email);

             if (!User.EmailConfirmed)
                throw new ForbiddenException("من فضلك فعّل بريدك الإلكتروني قبل تسجيل الدخول.");
            if (googleLoginDTo.DeviceToken is not null) // Consume
            {
                User.DeviceToken = googleLoginDTo.DeviceToken;
                await _userManager.UpdateAsync(User);
                await _geoLocationService.UpdateDonorLocationAndDeviceTokenAsync(User.Id.ToString(), User.DeviceToken, User.Longitude, User.Latitude, User.BloodTypeId.ToString());
            }
            return new AuthUserDTo
            {
                Email = User.Email!,
                FullName = User.FullName,
                Token = await CreateTokenAsync(User),
                CityName = User.City.NameAr,
                GovernorateName = User.City.Governorate.NameAr
            };
        }
        private async Task<string> CreateTokenAsync(BloodDonationUser user)
        {
            // Claim Specifications -> 
            var Claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("BloodTypeId", $"{user.BloodTypeId}"),
                //new Claim("CityId", $"{user.CityId}"),
                //new Claim("Governorate", user.City.Governorate.NameAr)
            };
            // Specify Cipher Algorithm
            var Creds = new SigningCredentials
                (
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:Key"]!)), // Key
                    SecurityAlgorithms.HmacSha256 // Security Algorithm
                );
            var Roles = await _userManager.GetRolesAsync(user);
            foreach (var item in Roles) // Add Roles To Claims
            {
                Claims.Add(new Claim(ClaimTypes.Role, item));
            }

            var TokenDescriptor = new SecurityTokenDescriptor // 
            {
                Subject = new ClaimsIdentity(Claims), // Informations
                Expires = DateTime.UtcNow.AddHours(double.Parse(_configuration["JWTSettings:ExpireDurationInHours"]!)),
                SigningCredentials = Creds,

                // 
                Issuer = _configuration["JWTSettings:Issuer"],
                Audience = _configuration["JWTSettings:Audience"],
            };

            var TokenHandler = new JwtSecurityTokenHandler();
            var Token = TokenHandler.CreateToken(TokenDescriptor); // Create
            return TokenHandler.WriteToken(Token); // Compress & Encapsulate & To String
        }
        public async Task SendForgetPasswordOTPAsync(ForgetPasswordDTo forgetPasswordDTo)
        {
            var User = await _userManager.FindByEmailAsync(forgetPasswordDTo.Email) ?? throw new UserNotFoundException(forgetPasswordDTo.Email);
            if (await _userManager.IsLockedOutAsync(User))
            {
                throw new UnauthorizedException("هذا الحساب محظور، يرجى التواصل مع الإدارة!");
            }
            var OTPCode = GenerateOTPCode();
            User.ResetCode = OTPCode;
            User.ExpireCodeTime = DateTime.UtcNow.AddMinutes(_settings.MinutesToExpireOTPCode);
            await _userManager.UpdateAsync(User);
            await _publishEndpoint.Publish(new SendEmailEvent()
            {
                Body = EmailSendHelper.GetForgetPasswordEmailBody(OTPCode),
                Subject = "كود إعادة تعيين كلمة المرور",
                To = forgetPasswordDTo.Email
            });
        }
        public async Task<ResetPasswordToken> VerifyForgetPasswordOTPAsync(VerifyOTPDTo verifyOTPDTo)
        {
            var User = await _userManager.FindByEmailAsync(verifyOTPDTo.Email) ?? throw new UserNotFoundException(verifyOTPDTo.Email);
            if (User.ResetCode is null || User.ResetCode != verifyOTPDTo.OTP || User.ExpireCodeTime < DateTime.UtcNow)
            {
                throw new UnauthorizedException("رمز التحقق لمرة واحدة غير صالح أو منتهي الصلاحية!");
            }
            User.ResetCode = null;
            User.ExpireCodeTime = null!;
            await _userManager.UpdateAsync(User);
            var token = await _userManager.GeneratePasswordResetTokenAsync(User);
            return new ResetPasswordToken() {
                ResetToken = token
            };
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDTo resetPasswordDTo)
        {
            var User = await _userManager.FindByEmailAsync(resetPasswordDTo.Email) ?? throw new UserNotFoundException(resetPasswordDTo.Email);
            var Result = await _userManager.ResetPasswordAsync(User, resetPasswordDTo.ResetToken, resetPasswordDTo.NewPassword);
            if (!Result.Succeeded)
            {
                List<string> Errors = Result.Errors.Select(E => E.Description).ToList();
                throw new BadRequestException(Errors);
            }
            return true;
        }
        private string GenerateOTPCode()
        {
            int OTP = RandomNumberGenerator.GetInt32(0, 1000000);
            return OTP.ToString("D6");
        }
        public async Task ConfirmEmail(ConfirmEmailDTo confirmEmailDTo)
        {
            var User = await _userManager.FindByEmailAsync(confirmEmailDTo.Email) ?? throw new UserNotFoundException(confirmEmailDTo.Email);
            var Token = Encoding.UTF8.GetString(
                Convert.FromBase64String(confirmEmailDTo.Token)
            );
            var Result = await _userManager.ConfirmEmailAsync(User, Token);
            if (!Result.Succeeded)
            {
                throw new BadRequestException(Result.Errors.Select(E => E.Description).ToList());
            }
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordDTo changePasswordDTo)
        {
            var UserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException("User is Unauthenticated!");
            var User = await _userManager.FindByIdAsync(UserId) ?? throw new UserNotFoundException(UserId);
            var Result = await _userManager.ChangePasswordAsync(User, changePasswordDTo.OldPassword, changePasswordDTo.NewPassword);
            if (!Result.Succeeded)
            {
                var Errors = Result.Errors.Select(E => E.Description).ToList();
                throw new BadRequestException(Errors);
            }
            return true;
        }
    }
}
