using MovieReview.Core.DTOs.User;
using MovieReview.Core.Interfaces.Repositories;
using MovieReview.Core.Interfaces.Services;

namespace MovieReview.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<UserProfileResponse> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        return new UserProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            ProfilePhoto = user.ProfilePhoto,
            Dob = user.Dob,
            Instagram = user.Instagram,
            Youtube = user.Youtube
        };
    }

    public async Task<UserProfileResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.ProfilePhoto = request.ProfilePhoto;
        user.Dob = request.Dob;
        user.Instagram = request.Instagram;
        user.Youtube = request.Youtube;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return new UserProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.ToString(),
            ProfilePhoto = user.ProfilePhoto,
            Dob = user.Dob,
            Instagram = user.Instagram,
            Youtube = user.Youtube
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ArgumentException("Current password is required", nameof(currentPassword));

        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("New password is required", nameof(newPassword));

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            throw new InvalidOperationException("Current password is incorrect");

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return true;
    }
}