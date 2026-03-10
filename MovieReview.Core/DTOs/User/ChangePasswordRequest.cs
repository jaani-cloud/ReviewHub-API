using MovieReview.Core.Validators;
using System.ComponentModel.DataAnnotations;

namespace MovieReview.Core.DTOs.User;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StrongPassword]
    public string NewPassword { get; set; } = string.Empty;
}