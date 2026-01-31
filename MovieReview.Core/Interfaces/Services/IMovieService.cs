using MovieReview.Core.DTOs.Movie;

namespace MovieReview.Core.Interfaces.Services;

public interface IMovieService
{
    Task<MovieResponse> GetMovieByIdAsync(int id);
    Task<IEnumerable<MovieResponse>> GetAllMoviesAsync();
    Task<IEnumerable<MovieResponse>> SearchMoviesAsync(string search);
    Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request);
    Task<MovieResponse> UpdateMovieAsync(int id, UpdateMovieRequest request);
    Task<bool> DeleteMovieAsync(int id);
}