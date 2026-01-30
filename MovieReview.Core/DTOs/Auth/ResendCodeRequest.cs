using System.ComponentModel.DataAnnotations;

namespace MovieReview.Core.DTOs.Auth;

public class ResendCodeRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}