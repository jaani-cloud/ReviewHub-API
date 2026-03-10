using System.ComponentModel.DataAnnotations;

namespace MovieReview.Core.DTOs.Movie;

public class UpdateMovieRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Poster { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2100)]
    public int ReleaseYear { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public List<string> Category { get; set; } = new();

    [Required]
    public List<string> Genre { get; set; } = new();
}