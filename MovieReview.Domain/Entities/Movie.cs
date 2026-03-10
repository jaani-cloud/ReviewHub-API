using MovieReview.Domain.Enums;

namespace MovieReview.Domain.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Poster { get; set; }
    public string Description { get; set; }
    public int ReleaseYear { get; set; }
    public MovieType Type { get; set; }
    public string Category { get; set; }
    public string Genre { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Review> Reviews { get; set; }
}