using MovieReview.Core.DTOs.User;
using MovieReview.Domain.Entities;

namespace MovieReview.Core.Interfaces.Services;

public interface IUserService
{
    Task<UserProfileResponse> GetProfileAsync(int userId);
    Task<UserProfileResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}