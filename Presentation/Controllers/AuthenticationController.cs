using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DataTransferObjects.Authentication;
using Shared.DataTransferObjects.Authentication.PasswordsAndOTPDTos;
using Shared.DataTransferObjects.Authentication.ResetForgetChangePasswordDTos;

namespace Presentation.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController(IServiceManager _serviceManager) : ApiBaseController
    {
        [HttpPost("login")]
        public async Task<ActionResult<AuthUserDTo>> Login(LoginDTo loginDTo)
        {
            return Ok(await _serviceManager.AuthenticationService.LoginAsync(loginDTo));
        }
        [HttpPost("google-login")]
        public async Task<ActionResult<AuthUserDTo>> GoogleLogin(GoogleLoginDTo googleLoginDTo)
        {
            return Ok(await _serviceManager.AuthenticationService.GoogleLoginAsync(googleLoginDTo));
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthRegisterDTo>> Register(RegisterDTo registerDTo)
        {
            return Ok(await _serviceManager.AuthenticationService.RegisterAsync(registerDTo));
        }
        [HttpPost("forget-otp")]
        public async Task<ActionResult> SendForgetPasswordOTP(ForgetPasswordDTo forgetPasswordDTo)
        {
            await _serviceManager.AuthenticationService.SendForgetPasswordOTPAsync(forgetPasswordDTo);
            return Ok(new { Message = "تم ارسال رمز التحقق" });
        }
        [HttpPost("resend-forget-otp")]
        public async Task<ActionResult> ResendForgetPasswordOTP(ForgetPasswordDTo forgetPasswordDTo)
        {
            await _serviceManager.AuthenticationService.SendForgetPasswordOTPAsync(forgetPasswordDTo);
            return Ok(new { Message = "تم ارسال رمز التحقق مرة آخرى" });
        }
        [HttpPost("verify-otp")]
        public async Task<ActionResult<ResetPasswordToken>> VerifyForgetPasswordOTP(VerifyOTPDTo verifyOTPDTo)
        {
            return Ok(await _serviceManager.AuthenticationService.VerifyForgetPasswordOTPAsync(verifyOTPDTo));
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult<bool>> ResetPasswordAsync(ResetPasswordDTo resetPasswordDTo)
        {
            return Ok(await _serviceManager.AuthenticationService.ResetPasswordAsync(resetPasswordDTo));
        }
        [HttpGet("confirm-email", Name = "confirm-email")]
        public async Task<IActionResult> ConfrimEmail(string Email, string Token)
        {
            try
            {
                await _serviceManager.AuthenticationService.ConfirmEmail(new ConfirmEmailDTo() { Email = Email, Token = Token });
                return Content(GetSuccessPage(), "text/html");
            }
            catch
            {
                return Content(GetErrorPage("رابط غير صالح أو منتهي الصلاحية!"), "text/html");
            }
        }
        [HttpPost("change-password")]
        public async Task<ActionResult<bool>> ChangePasswordAsync(ChangePasswordDTo changePasswordDTo)
        {
            return Ok(await _serviceManager.AuthenticationService.ChangePasswordAsync(changePasswordDTo));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthUserDTo>> RefreshToken(RefreshTokenDTo refreshTokenDTo)
        {
            return Ok(await _serviceManager.AuthenticationService.RefreshTokenAsync(refreshTokenDTo));
        }
        private string GetSuccessPage() => @"
                                        <!DOCTYPE html>
                                        <html lang='ar' dir='rtl'>
                                        <head>
                                        <meta charset='UTF-8'>
                                        <title>تم التفعيل</title>
                                        <style>
                                        body {
                                            font-family: Segoe UI, Tahoma, Arial, sans-serif;
                                            background:#f5f5f5;
                                            display:flex;
                                            justify-content:center;
                                            align-items:center;
                                            height:100vh;
                                            margin:0;
                                        }
                                        .card {
                                            background:#fff;
                                            padding:40px;
                                            border-radius:16px;
                                            box-shadow:0 10px 30px rgba(0,0,0,0.1);
                                            text-align:center;
                                        }
                                        h1 { color:#b71c1c; }
                                        p { color:#555; font-size:16px; }
                                        </style>
                                        </head>
                                        <body>
                                        <div class='card'>
                                            <h1>✅ تم تفعيل حسابك</h1>
                                            <p>يمكنك الآن تسجيل الدخول إلى التطبيق</p>
                                        </div>
                                        </body>
                                        </html>";
        private string GetErrorPage(string message) => $@"
                                        <!DOCTYPE html>
                                        <html lang='ar' dir='rtl'>
                                        <head>
                                        <meta charset='UTF-8'>
                                        <title>خطأ</title>
                                        <style>
                                        body {{
                                            font-family: Segoe UI, Tahoma, Arial, sans-serif;
                                            background:#f5f5f5;
                                            display:flex;
                                            justify-content:center;
                                            align-items:center;
                                            height:100vh;
                                        }}
                                        .card {{
                                            background:#fff;
                                            padding:40px;
                                            border-radius:16px;
                                            box-shadow:0 10px 30px rgba(0,0,0,0.1);
                                            text-align:center;
                                        }}
                                        h1 {{ color:#b71c1c; }}
                                        </style>
                                        </head>
                                        <body>
                                        <div class='card'>
                                            <h1>❌ فشل التفعيل</h1>
                                            <p>{message}</p>
                                        </div>
                                        </body>
                                        </html>";
    }
}
