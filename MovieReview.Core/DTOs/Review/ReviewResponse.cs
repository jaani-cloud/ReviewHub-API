namespace MovieReview.Core.DTOs.Review;

public class ReviewResponse
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public string MovieName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserPhoto { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public decimal Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}