using MovieReview.Domain.Enums;

namespace MovieReview.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public int UserId { get; set; }
    public ReviewType Type { get; set; }
    public string? Comment { get; set; }
    public decimal Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Movie Movie { get; set; }
    public User User { get; set; }
}