using Shared.DataTransferObjects.Authentication;
using Shared.DataTransferObjects.Authentication.PasswordsAndOTPDTos;
using Shared.DataTransferObjects.Authentication.ResetForgetChangePasswordDTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Abstractions
{
    public interface IAuthenticationService
    {
        // Register
        // Take Email, FullName, BloodTypeId, Age, Gender, Latitude, Longitude, CityId
        // Return Token, Email, Display Name
        Task<AuthRegisterDTo> RegisterAsync(RegisterDTo registerDTo);
        // Login
        // Take Email And Password, Return Token, Display Name
        Task<AuthUserDTo> LoginAsync(LoginDTo loginDTo);
        Task SendForgetPasswordOTPAsync(ForgetPasswordDTo forgetPasswordDTo);
        Task<ResetPasswordToken> VerifyForgetPasswordOTPAsync(VerifyOTPDTo verifyOTPDTo);
        Task<bool> ResetPasswordAsync(ResetPasswordDTo resetPasswordDTo);
        Task ConfirmEmail(ConfirmEmailDTo confirmEmailDTo);
        Task<AuthUserDTo> GoogleLoginAsync(GoogleLoginDTo googleLoginDTo);
        public Task<bool> ChangePasswordAsync(ChangePasswordDTo changePasswordDTo);
        public Task<NewRefreshTokenDTo> RefreshTokenAsync(RefreshTokenDTo model);
    }
}
