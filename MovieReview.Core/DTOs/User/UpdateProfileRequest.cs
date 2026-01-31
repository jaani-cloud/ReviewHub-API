using System.ComponentModel.DataAnnotations;

namespace MovieReview.Core.DTOs.User;

public class UpdateProfileRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 10)]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? ProfilePhoto { get; set; }
    public DateTime? Dob { get; set; }
    public string? Instagram { get; set; }
    public string? Youtube { get; set; }
}