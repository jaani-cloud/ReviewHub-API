namespace MovieReview.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendVerificationEmailAsync(string email, string code, string firstName);
    Task SendPasswordResetEmailAsync(string email, string code);
}