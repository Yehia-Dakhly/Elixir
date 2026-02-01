using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Shared;

namespace Service
{
    public static class EmailSendHelper
    {
        private static readonly IConfigurationRoot _config;

        static EmailSendHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Directory.GetCurrentDirectory()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _config = builder.Build();
        }

        public static async Task SendEmailAsync(Email email)
        {
            var fromName = _config["EmailSettings:AppName"];
            var fromAddress = _config["EmailSettings:Email"];
            var appPassword = _config["EmailSettings:Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.Add(MailboxAddress.Parse(email.To));
            message.Subject = email.Subject;
            message.Body = new TextPart("html") { Text = email.Body };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(fromAddress, appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        public static string GetForgetPasswordEmailBody(string OTP)
        {
            return $@"
                    <html lang='ar' dir='rtl'>
                    <body style='margin:0; padding:0; background-color:#f5f5f5; font-family:Segoe UI, Tahoma, sans-serif;'>

                      <div style='max-width:600px; margin:40px auto; background:#ffffff; border-radius:16px; overflow:hidden; box-shadow:0 10px 30px rgba(0,0,0,0.1);'>

                        <!-- Header -->
                        <div style='background:#b71c1c; padding:30px; text-align:center;'>
                          <h2 style='color:#ffffff; margin:0;'>استعادة كلمة المرور</h2>
                        </div>

                        <!-- Content -->
                        <div style='padding:30px; color:#333;'>
                          <p style='font-size:16px;'>مرحباً 👋</p>

                          <p style='font-size:15px; line-height:1.7; color:#555;'>
                            تلقينا طلبًا لإعادة تعيين كلمة المرور الخاصة بحسابك.
                            استخدم رمز التحقق التالي لإكمال العملية:
                          </p>

                          <!-- OTP -->
                          <div style='text-align:center; margin:30px 0;'>
                            <span style='display:inline-block; background:#fdecea; color:#b71c1c; padding:16px 32px;
                                         font-size:26px; font-weight:bold; letter-spacing:6px;
                                         border-radius:12px; border:1px dashed #b71c1c;'>
                              {OTP}
                            </span>
                          </div>

                          <p style='font-size:14px; color:#777; line-height:1.6;'>
                            إذا لم تقم بطلب إعادة تعيين كلمة المرور، يمكنك تجاهل هذه الرسالة بأمان.
                          </p>
                        </div>

                        <!-- Footer -->
                        <div style='background:#fafafa; padding:20px; text-align:center; font-size:12px; color:#999;'>
                          نتمنى لك دوام الصحة والعافية ❤️<br>
                          فريق التطبيق
                        </div>

                      </div>

                    </body>
                    </html>
                    ";
        }
        public static string GetConfirmRegisterEmailBody(string ConfirmLink)
        {
            return $@"
                    <html lang='ar' dir='rtl'>
                    <body style='margin:0; padding:0; background-color:#f5f5f5; font-family:Segoe UI, Tahoma, sans-serif;'>

                      <div style='max-width:600px; margin:40px auto; background:#ffffff; border-radius:16px;
                                  overflow:hidden; box-shadow:0 10px 30px rgba(0,0,0,0.1);'>

                        <!-- Header -->
                        <div style='background:#b71c1c; padding:30px; text-align:center;'>
                          <h2 style='color:#ffffff; margin:0;'>تأكيد البريد الإلكتروني</h2>
                        </div>

                        <!-- Content -->
                        <div style='padding:30px; color:#333;'>
                          <p style='font-size:16px;'>مرحباً 👋</p>

                          <p style='font-size:15px; line-height:1.7; color:#555;'>
                            شكرًا لإنشائك حسابًا معنا 🎉  
                            لتفعيل حسابك، يرجى تأكيد بريدك الإلكتروني عبر الضغط على الزر التالي:
                          </p>

                          <!-- Button -->
                          <div style='text-align:center; margin:35px 0;'>
                            <a href='{ConfirmLink}'
                               style='background:#b71c1c; color:#ffffff; text-decoration:none;
                                      padding:14px 36px; font-size:16px; font-weight:bold;
                                      border-radius:30px; display:inline-block;'>
                              تأكيد البريد الإلكتروني
                            </a>
                          </div>

                          <p style='font-size:14px; color:#777; line-height:1.6;'>
                            إذا لم تقم بإنشاء هذا الحساب، يمكنك تجاهل هذه الرسالة.
                          </p>

                          <p style='font-size:13px; color:#999; margin-top:20px;'>
                            أو انسخ الرابط التالي في المتصفح:
                            <br>
                            <span style='word-break:break-all; color:#b71c1c;'>{ConfirmLink}</span>
                          </p>
                        </div>

                        <!-- Footer -->
                        <div style='background:#fafafa; padding:20px; text-align:center; font-size:12px; color:#999;'>
                          سعداء بانضمامك إلينا ❤️<br>
                          فريق التطبيق
                        </div>

                      </div>

                    </body>
                    </html>
                    ";
        }
    }
}
