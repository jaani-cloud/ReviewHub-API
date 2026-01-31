namespace MovieReview.Core.DTOs.User;

public class UserProfileResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ProfilePhoto { get; set; }
    public DateTime? Dob { get; set; }
    public string? Instagram { get; set; }
    public string? Youtube { get; set; }
}