using MovieReview.Domain.Entities;

namespace MovieReview.Core.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
}