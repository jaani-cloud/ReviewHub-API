using MovieReview.Core.DTOs.User;

public interface IUserService
{
    Task<UserProfileResponse> GetProfileAsync(int userId);
    Task<UserProfileResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<IEnumerable<UserProfileResponse>> GetAllUsersAsync();
}
