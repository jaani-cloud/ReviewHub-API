using System.ComponentModel.DataAnnotations;
using MovieReview.Core.Validators;

namespace MovieReview.Core.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(15, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 15 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(15, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 15 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StrongPassword]
    public string Password { get; set; } = string.Empty;
}