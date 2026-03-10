using System.ComponentModel.DataAnnotations;

namespace MovieReview.Core.DTOs.Review;

public class CreateReviewRequest
{
    [Required]
    public int MovieId { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    public string? Comment { get; set; }
}