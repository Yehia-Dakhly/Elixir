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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Specifications;
using ServiceAbstraction;
using Shared.Constants;
using Shared.DataTransferObjects.Authentication;
using Shared.DataTransferObjects.Authentication.PasswordsAndOTPDTos;
using Shared.DataTransferObjects.Authentication.ResetForgetChangePasswordDTos;
using Shared.Events;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        ILogger<AuthenticationService> _logger,
        IGeoLocationService _geoLocationService) : IAuthenticationService
    {
        private readonly BloodDonationSettings _settings = _optionsSnapshot.Value;
        public async Task<AuthRegisterDTo> RegisterAsync(RegisterDTo registerDTo) // Age Constraint
        {
            _logger.LogInformation("An attempt to register for email: {Email}.", registerDTo.Email);
            Gender Gender = registerDTo.Gender switch
            {
                1 => Gender.Male,
                2 => Gender.Female,
                _ => Gender.Undefined
            };
            var NewUser = new BloodDonationUser()
            {
                FullName = registerDTo.FullName,
                Email = registerDTo.Email,
                DateOfBirth = registerDTo.DateOfBirth,
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
                await _userManager.AddToRoleAsync(NewUser, ElixirRoles.User);
                var contxt = httpContextAccessor.HttpContext;
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
                _logger.LogInformation("New user registered: {Email}", NewUser.Email);
                return new AuthRegisterDTo()
                {
                    Email = registerDTo.Email,
                    FullName = registerDTo.FullName,
                };
            }
            else
            {
                var Errors = Result.Errors.Select(E => E.Description).ToList();
                _logger.LogWarning("Failed registration attempt for email: {Email}. Errors: {Errors}", registerDTo.Email, string.Join(", ", Errors));
                throw new BadRequestException(Errors);
            }
        }
        public async Task<AuthUserDTo> LoginAsync(LoginDTo loginDTo)
        {
            var User = await _userManager.Users.Include(U => U.City).ThenInclude(C => C.Governorate).Include(U => U.RefreshTokens).FirstOrDefaultAsync(U => U.Email == loginDTo.Email) ?? throw new UserNotFoundException(loginDTo.Email);
            if (await _userManager.IsLockedOutAsync(User))
            {
                _logger.LogWarning("Locked out login attempt for email: {Email}", loginDTo.Email);
                throw new UnauthorizedException("هذا الحساب محظور، يرجى التواصل مع الإدارة!");
            }
            if (await _userManager.IsEmailConfirmedAsync(User) == false)
            {
                _logger.LogWarning("Unconfirmed email login attempt for email: {Email}", loginDTo.Email);
                throw new ForbiddenException("من فضلك فعّل بريدك الإلكتروني قبل تسجيل الدخول.");
            }
            if (await _userManager.CheckPasswordAsync(User, loginDTo.Password))
            {
                _logger.LogInformation("Successful login for email: {Email}", loginDTo.Email);
                await _userManager.ResetAccessFailedCountAsync(User);
                if (loginDTo.DeviceToken is not null) // Consume
                {
                    _logger.LogInformation("Updating device token for user: {Email}", loginDTo.Email);
                    User.DeviceToken = loginDTo.DeviceToken;
                    await _userManager.UpdateAsync(User);
                    await _geoLocationService.UpdateDonorLocationAndDeviceTokenAsync(User.Id.ToString(), User.DeviceToken, User.Longitude, User.Latitude, User.BloodTypeId.ToString());
                }
                #region Refresh Token
                var rawRefreshToken = GenerateSecureToken();
                var refreshTokenHash = ComputeSha256Hash(rawRefreshToken);

                var refreshTokenEntity = new RefreshToken
                {
                    TokenHash = refreshTokenHash,
                    ExpiresOn = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifespanInDays),
                    CreatedOn = DateTime.UtcNow
                };
                User.RefreshTokens.Add(refreshTokenEntity);
                await _userManager.UpdateAsync(User);
                #endregion

                return new AuthUserDTo // With Refresh Token
                {
                    Email = User.Email!,
                    FullName = User.FullName,
                    Token = await CreateTokenAsync(User),
                    CityName = User.City.NameAr,
                    GovernorateName = User.City.Governorate.NameAr,
                    RefreshToken = rawRefreshToken,
                    RefreshTokenExpiration = refreshTokenEntity.ExpiresOn,
                    BloodTypeId = User.BloodTypeId,
                };
            }
            else
            {
                await _userManager.AccessFailedAsync(User);
                _logger.LogWarning("Failed login attempt for email: {Email}. Incorrect password.", loginDTo.Email);
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
            catch (InvalidJwtException ex)
            {
                _logger.LogWarning(ex, "Google token validation failed. The provided token is invalid, malformed, or expired.");
                throw new UnauthorizedException("Invalid Google Token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A system error occurred while validating Google token. Possible network issue or Google API outage.");
                throw new InvalidOperationException("Authentication service is temporarily unavailable. Please try again later.");
            }
            var User = await _userManager.Users.Include(U => U.City).ThenInclude(C => C.Governorate).FirstOrDefaultAsync(U => U.Email == payload.Email) ?? throw new UserNotFoundException(payload.Email);

            if (!User.EmailConfirmed)
            {
                _logger.LogWarning("Unconfirmed email login attempt for email: {Email} via Google. Email not confirmed.", payload.Email);
                throw new ForbiddenException("من فضلك فعّل بريدك الإلكتروني قبل تسجيل الدخول.");
            }
            if (googleLoginDTo.DeviceToken is not null) // Consume
            {
                _logger.LogInformation("Updating device token for user: {Email} via Google login.", payload.Email);
                User.DeviceToken = googleLoginDTo.DeviceToken;
                await _userManager.UpdateAsync(User);
                await _geoLocationService.UpdateDonorLocationAndDeviceTokenAsync(User.Id.ToString(), User.DeviceToken, User.Longitude, User.Latitude, User.BloodTypeId.ToString());
            }

            #region Refresh Token
            var rawRefreshToken = GenerateSecureToken();
            var refreshTokenHash = ComputeSha256Hash(rawRefreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                ExpiresOn = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifespanInDays),
                CreatedOn = DateTime.UtcNow
            };
            User.RefreshTokens.Add(refreshTokenEntity);
            await _userManager.UpdateAsync(User);
            #endregion
            _logger.LogInformation("Successful Google login for email: {Email}", payload.Email);
            return new AuthUserDTo // With Refresh Token
            {
                Email = User.Email!,
                FullName = User.FullName,
                Token = await CreateTokenAsync(User),
                CityName = User.City.NameAr,
                GovernorateName = User.City.Governorate.NameAr,
                RefreshToken = rawRefreshToken,
                RefreshTokenExpiration = refreshTokenEntity.ExpiresOn,
                BloodTypeId = User.BloodTypeId,
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
                new Claim("ExpireDate", $"{DateTime.UtcNow.AddHours(double.Parse(_configuration["JWTSettings:ExpireDurationInHours"]!))}"),
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
            _logger.LogInformation("Password reset requested for email: {Email}", forgetPasswordDTo.Email);
            var User = await _userManager.FindByEmailAsync(forgetPasswordDTo.Email) ?? throw new UserNotFoundException(forgetPasswordDTo.Email);
            if (await _userManager.IsLockedOutAsync(User))
            {
                _logger.LogWarning("Locked out password reset attempt for email: {Email}", forgetPasswordDTo.Email);
                throw new UnauthorizedException("هذا الحساب محظور، يرجى التواصل مع الإدارة!");
            }
            var OTPCode = GenerateOTPCode();
            User.ResetCode = OTPCode;
            User.ExpireCodeTime = DateTime.UtcNow.AddMinutes(_settings.MinutesToExpireOTPCode);
            await _userManager.UpdateAsync(User);
            _logger.LogInformation("Generated OTP code for email: {Email}. OTP expires at: {ExpireTime}", forgetPasswordDTo.Email, User.ExpireCodeTime);
            await _publishEndpoint.Publish(new SendEmailEvent()
            {
                Body = EmailSendHelper.GetForgetPasswordEmailBody(OTPCode),
                Subject = "كود إعادة تعيين كلمة المرور",
                To = forgetPasswordDTo.Email
            });
        }
        public async Task<ResetPasswordToken> VerifyForgetPasswordOTPAsync(VerifyOTPDTo verifyOTPDTo)
        {
            _logger.LogInformation("Verifying OTP for email: {Email}", verifyOTPDTo.Email);
            var User = await _userManager.FindByEmailAsync(verifyOTPDTo.Email) ?? throw new UserNotFoundException(verifyOTPDTo.Email);
            if (User.ResetCode is null || User.ResetCode != verifyOTPDTo.OTP || User.ExpireCodeTime < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired OTP verification attempt for email: {Email}", verifyOTPDTo.Email);
                throw new UnauthorizedException("رمز التحقق لمرة واحدة غير صالح أو منتهي الصلاحية!");
            }
            User.ResetCode = null;
            User.ExpireCodeTime = null!;
            await _userManager.UpdateAsync(User);
            var token = await _userManager.GeneratePasswordResetTokenAsync(User);
            _logger.LogInformation("OTP verified successfully for email: {Email}. Generated password reset token.", verifyOTPDTo.Email);
            return new ResetPasswordToken()
            {
                ResetToken = token
            };
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDTo resetPasswordDTo)
        {
            _logger.LogInformation("Attempting to reset password for email: {Email}", resetPasswordDTo.Email);
            var User = await _userManager.FindByEmailAsync(resetPasswordDTo.Email) ?? throw new UserNotFoundException(resetPasswordDTo.Email);
            var Result = await _userManager.ResetPasswordAsync(User, resetPasswordDTo.ResetToken, resetPasswordDTo.NewPassword);
            if (!Result.Succeeded)
            {
                List<string> Errors = Result.Errors.Select(E => E.Description).ToList();
                _logger.LogWarning("Failed password reset attempt for email: {Email}. Errors: {Errors}", resetPasswordDTo.Email, string.Join(", ", Errors));
                throw new BadRequestException(Errors);
            }
            _logger.LogInformation("Password reset successfully for email: {Email}", resetPasswordDTo.Email);
            return true;
        }
        private string GenerateOTPCode()
        {
            int OTP = RandomNumberGenerator.GetInt32(0, 1000000);
            return OTP.ToString("D6");
        }
        public async Task ConfirmEmail(ConfirmEmailDTo confirmEmailDTo)
        {
            _logger.LogInformation("Attempting to confirm email: {Email}", confirmEmailDTo.Email);
            var User = await _userManager.FindByEmailAsync(confirmEmailDTo.Email) ?? throw new UserNotFoundException(confirmEmailDTo.Email);
            var Token = Encoding.UTF8.GetString(
                Convert.FromBase64String(confirmEmailDTo.Token)
            );
            var Result = await _userManager.ConfirmEmailAsync(User, Token);
            if (!Result.Succeeded)
            {
                _logger.LogWarning("Failed email confirmation attempt for email: {Email}. Errors: {Errors}", confirmEmailDTo.Email, string.Join(", ", Result.Errors.Select(E => E.Description)));
                throw new BadRequestException(Result.Errors.Select(E => E.Description).ToList());
            }
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordDTo changePasswordDTo)
        {
            _logger.LogInformation("Attempting to change password for user with ID: {UserId}", httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var UserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException("User is Unauthenticated!");
            var User = await _userManager.FindByIdAsync(UserId) ?? throw new UserNotFoundException(UserId);
            var Result = await _userManager.ChangePasswordAsync(User, changePasswordDTo.OldPassword, changePasswordDTo.NewPassword);
            if (!Result.Succeeded)
            {
                var Errors = Result.Errors.Select(E => E.Description).ToList();
                _logger.LogWarning("Failed password change attempt for user with ID: {UserId}. Errors: {Errors}", UserId, string.Join(", ", Errors));
                throw new BadRequestException(Errors);
            }
            _logger.LogInformation("Password changed successfully for user with ID: {UserId}", UserId);
            return true;
        }


        #region Generate and Hash Refresh Token
        private string GenerateSecureToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }
        #endregion


        public async Task<NewRefreshTokenDTo> RefreshTokenAsync(RefreshTokenDTo model)
        {
            var principal = GetPrincipalFromExpiredToken(model.Token);
            if (principal == null)
            {
                _logger.LogWarning("Invalid token refresh attempt. Unable to extract principal from expired token.");
                throw new ForbiddenException("رمز وصول غير صالح");
            }

            var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) ?? throw new UserNotFoundException("لا يوجد مستخدم بهذا البريد الإلكتروني!");

            var email = emailClaim.Value;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            { 
                _logger.LogWarning("Token refresh attempt failed. No user found with email: {Email}", email);
                throw new UserNotFoundException(email); 
            }

            var incomingTokenHash = ComputeSha256Hash(model.RefreshToken);

            var userRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.TokenHash == incomingTokenHash);

            if (userRefreshToken == null || !userRefreshToken.IsActive)
            {
                _logger.LogWarning("Invalid or expired refresh token attempt for user with email: {Email}", email);
                throw new ForbiddenException("رمز التحديث غير صالح أو منتهي الصلاحية، يرجى تسجيل الدخول مرة أخرى.");
            }

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var newJwtToken = await CreateTokenAsync(user);

            var newRawRefreshToken = GenerateSecureToken();
            var newRefreshTokenHash = ComputeSha256Hash(newRawRefreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                TokenHash = newRefreshTokenHash,
                ExpiresOn = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifespanInDays),
                CreatedOn = DateTime.UtcNow,
            };

            user.RefreshTokens.Add(newRefreshTokenEntity);
            await _userManager.UpdateAsync(user);
            _logger.LogInformation("Refresh token successfully renewed for user with email: {Email}", email);
            return new NewRefreshTokenDTo
            {
                Token = newJwtToken,
                RefreshToken = newRawRefreshToken,
                RefreshTokenExpiration = newRefreshTokenEntity.ExpiresOn
            };
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:Key"]!)),

                ValidateIssuer = false,
                ValidIssuer = _configuration["JWTSettings:Issuer"],

                ValidateAudience = false,
                ValidAudience = _configuration["JWTSettings:Audience"],

                ValidateLifetime = false // We want to get claims from expired token, so we don't validate lifetime
            };
            //Console.WriteLine(_configuration["JWTSettings:Issuer"]);
            //Console.WriteLine(_configuration["JWTSettings:Audience"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            //var jwtToken = tokenHandler.ReadJwtToken(token);
            //Console.WriteLine("Token Issuer from JWT: " + jwtToken.Issuer);
            //Console.WriteLine("Token Audience: " + jwtToken.Audiences.FirstOrDefault());
            //Console.WriteLine("Token claims: " + string.Join(", ", jwtToken.Claims.Select(c => c.Type + "=" + c.Value)));
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Invalid token claims during refresh token validation. Token is not a valid JWT or does not use the expected signing algorithm.");
                throw new SecurityTokenException("Invalid Token Claims");
            }

            return principal;
        }
    }
}
