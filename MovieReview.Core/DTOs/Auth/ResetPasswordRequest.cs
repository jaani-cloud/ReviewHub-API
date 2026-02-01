using MovieReview.Core.Validators;
using System.ComponentModel.DataAnnotations;

namespace MovieReview.Core.DTOs.Auth;

public class ResetPasswordRequest
{
    [Required]
    public string ResetToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StrongPassword]
    public string NewPassword { get; set; } = string.Empty;
}