using MovieReview.Domain.Enums;

namespace MovieReview.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; } = Role.User;
    public bool EmailVerified { get; set; } = false;
    public string? ProfilePhoto { get; set; }
    public DateTime? Dob { get; set; }
    public string? Instagram { get; set; }
    public string? Youtube { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Review> Reviews { get; set; }
}