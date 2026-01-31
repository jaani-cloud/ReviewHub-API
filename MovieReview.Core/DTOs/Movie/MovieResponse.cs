using MovieReview.Domain.Enums;

namespace MovieReview.Core.DTOs.Movie;

public class MovieResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Type { get; set; } = string.Empty;
    public List<string> Category { get; set; } = new();
    public List<string> Genre { get; set; } = new();
    public decimal? AvgRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime CreatedAt { get; set; }
}