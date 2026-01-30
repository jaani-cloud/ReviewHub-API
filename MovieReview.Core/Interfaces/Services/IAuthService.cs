using MovieReview.Domain.Entities;

namespace MovieReview.Core.Interfaces.Services;

public interface IAuthService
{
    Task<bool> RegisterAsync(string firstName, string lastName, string email,
        string phoneNumber, string password);
    Task<string> LoginAsync(string email, string password);
    Task<bool> VerifyEmailAsync(string email, string code);
    Task<bool> ResendVerificationCodeAsync(string email);
}