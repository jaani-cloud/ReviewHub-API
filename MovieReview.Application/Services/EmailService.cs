using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MovieReview.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace MovieReview.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Email address is required", nameof(to));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject is required", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body is required", nameof(body));

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["Email:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(
                    _configuration["Email:Host"],
                    int.Parse(_configuration["Email:Port"] ?? "587"),
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]);

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }

        public async Task SendVerificationEmailAsync(string email, string code, string firstName)
        {
            var subject = "Verify Your Email - ReviewHub";
            var body = GenerateEmailTemplate(
                title: "Verify Email - ReviewHub",
                heading: $@"Welcome to ReviewHub<span style=""color:#1A1AFF; font-size:32px; font-weight:800;""><br>{firstName} Ji!</span>",
                icon: "🎬",
                message: "Thank you for joining ReviewHub! To complete your registration, please verify your email address using the code below:",
                code: code,
                codeLabel: "YOUR VERIFICATION CODE",
                expiryMinutes: 10,
                footerNote: "If you didn't create an account with ReviewHub, please ignore this email."
            );

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string code, string firstName)
        {
            var subject = "Reset Your Password - ReviewHub";
            var body = GenerateEmailTemplate(
                title: "Password Reset - ReviewHub",
                heading: $@"Reset Your Password<span style=""color:#1A1AFF; font-size:32px; font-weight:800;""><br>{firstName} Ji!</span>",
                icon: "🔒",
                message: "We received a request to reset your password. Use the code below to proceed with resetting your password:",
                code: code,
                codeLabel: "YOUR RESET CODE",
                expiryMinutes: 10,
                footerNote: "If you didn't request a password reset, please secure your account immediately by changing your password."
            );

            await SendEmailAsync(email, subject, body);
        }

        private string GenerateEmailTemplate(
            string title,
            string heading,
            string icon,
            string message,
            string code,
            string codeLabel,
            int expiryMinutes,
            string footerNote)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{title}</title>
</head>
<body style=""margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; min-height: 100vh;"">
        <tr>
            <td style=""padding: 2px;"">
                <!-- Main Container -->
                <table role=""presentation"" style=""max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 0; overflow: hidden;"">
                    
                    <!-- Header - Clean without box/shadow -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 35px 20px; text-align: center;"">
                            <div style=""font-size: 48px; margin-bottom: 12px;"">{icon}</div>
                            <h1 style=""margin: 0; color: #ffffff; font-size: 24px; font-weight: 700; letter-spacing: 0.5px;"">
                                ReviewHub
                            </h1>
                            <p style=""margin: 8px 0 0; color: rgba(255, 255, 255, 0.9); font-size: 13px; font-weight: 400;"">Your Movie Review Platform</p>
                        </td>
                    </tr>

                    <!-- Content -->
                    <tr>
                        <td style=""padding: 35px 25px;"">
                            <h2 style=""margin: 0 0 20px; color: #1a1a1a; font-size: 22px; font-weight: 600; text-align: center;"">{heading}</h2>
                            
                            <p style=""margin: 0 0 30px; color: #4a5568; font-size: 15px; line-height: 1.7; text-align: center;"">
                                {message}
                            </p>

                            <!-- Code Display Box -->
                            <div style=""background: linear-gradient(135deg, #f7fafc 0%, #edf2f7 100%); border: 2px dashed #cbd5e0; border-radius: 12px; padding: 25px; text-align: center; margin: 30px 0;"">
                                <p style=""margin: 0 0 12px; color: #718096; font-size: 11px; text-transform: uppercase; letter-spacing: 1.5px; font-weight: 700;"">{codeLabel}</p>
                                
                                <div style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 10px; padding: 20px; margin: 15px 0;"">
                                    <p style=""margin: 0; color: #ffffff; font-size: 32px; font-weight: 800; letter-spacing: 6px; font-family: 'Courier New', Consolas, monospace; text-shadow: 0 2px 4px rgba(0,0,0,0.2); user-select: all; -webkit-user-select: all; -moz-user-select: all; -ms-user-select: all;"">{code}</p>
                                </div>

                                <!-- Mobile-friendly Copy Instructions -->
                                <p style=""margin: 12px 0 0; color: #718096; font-size: 13px; font-weight: 500;"">Hold the code above to copy</p>
                            </div>

                            <!-- Expiry Warning -->
                            <div style=""background: linear-gradient(135deg, #fff5f5 0%, #fed7d7 100%); border-left: 4px solid #fc8181; padding: 16px 18px; margin: 25px 0; border-radius: 8px;"">
                                <p style=""margin: 0; color: #742a2a; font-size: 13px; line-height: 1.6;"">
                                    <strong style=""font-size: 16px;"">⏰</strong> This code will expire in <strong>{expiryMinutes} minutes</strong> for security reasons.
                                </p>
                            </div>

                            <!-- Security Notice -->
                            <div style=""background: linear-gradient(135deg, #ebf8ff 0%, #bee3f8 100%); border-left: 4px solid #4299e1; padding: 16px 18px; margin: 20px 0; border-radius: 8px;"">
                                <p style=""margin: 0; color: #2c5282; font-size: 13px; line-height: 1.6;"">
                                    <strong style=""font-size: 16px;"">🔐</strong> Never share this code with anyone. ReviewHub staff will never ask for your verification code.
                                </p>
                            </div>

                            <!-- Footer Note -->
                            <p style=""margin: 25px 0 0; color: #a0aec0; font-size: 13px; line-height: 1.6; text-align: center; padding-top: 20px; border-top: 1px solid #e2e8f0;"">
                                {footerNote}
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""background: linear-gradient(180deg, #f7fafc 0%, #edf2f7 100%); padding: 25px; text-align: center; border-top: 1px solid #e2e8f0;"">
                            <p style=""margin: 0 0 8px; color: #4a5568; font-size: 13px; font-weight: 500;"">
                                © 2025 ReviewHub. All rights reserved.
                            </p>
                            <p style=""margin: 0; color: #a0aec0; font-size: 12px;"">
                                This is an automated email. Please do not reply.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}