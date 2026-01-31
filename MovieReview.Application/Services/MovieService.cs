using MovieReview.Core.DTOs.Movie;
using MovieReview.Core.Interfaces.Repositories;
using MovieReview.Core.Interfaces.Services;
using MovieReview.Domain.Entities;
using MovieReview.Domain.Enums;
using System.Text.Json;

namespace MovieReview.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IReviewRepository _reviewRepository;

    public MovieService(IMovieRepository movieRepository, IReviewRepository reviewRepository)
    {
        _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
    }

    public async Task<MovieResponse> GetMovieByIdAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
            throw new InvalidOperationException("Movie not found");

        return await MapToResponseAsync(movie);
    }

    public async Task<IEnumerable<MovieResponse>> GetAllMoviesAsync()
    {
        var movies = await _movieRepository.GetAllAsync();
        var responses = new List<MovieResponse>();

        foreach (var movie in movies)
        {
            responses.Add(await MapToResponseAsync(movie));
        }

        return responses;
    }

    public async Task<IEnumerable<MovieResponse>> SearchMoviesAsync(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return await GetAllMoviesAsync();

        var movies = await _movieRepository.SearchMoviesAsync(search);
        var responses = new List<MovieResponse>();

        foreach (var movie in movies)
        {
            responses.Add(await MapToResponseAsync(movie));
        }

        return responses;
    }

    public async Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request)
    {
        if (!Enum.TryParse<MovieType>(request.Type, true, out var movieType))
            throw new ArgumentException("Invalid movie type. Must be 'Movie' or 'WebSeries'");

        var movie = new Movie
        {
            Name = request.Name,
            Poster = request.Poster,
            Description = request.Description,
            ReleaseYear = request.ReleaseYear,
            Type = movieType,
            Category = JsonSerializer.Serialize(request.Category),
            Genre = JsonSerializer.Serialize(request.Genre),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdMovie = await _movieRepository.AddAsync(movie);
        return await MapToResponseAsync(createdMovie);
    }

    public async Task<MovieResponse> UpdateMovieAsync(int id, UpdateMovieRequest request)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
            throw new InvalidOperationException("Movie not found");

        if (!Enum.TryParse<MovieType>(request.Type, true, out var movieType))
            throw new ArgumentException("Invalid movie type. Must be 'Movie' or 'WebSeries'");

        movie.Name = request.Name;
        movie.Poster = request.Poster;
        movie.Description = request.Description;
        movie.ReleaseYear = request.ReleaseYear;
        movie.Type = movieType;
        movie.Category = JsonSerializer.Serialize(request.Category);
        movie.Genre = JsonSerializer.Serialize(request.Genre);
        movie.UpdatedAt = DateTime.UtcNow;

        await _movieRepository.UpdateAsync(movie);
        return await MapToResponseAsync(movie);
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
            throw new InvalidOperationException("Movie not found");

        await _movieRepository.DeleteAsync(id);
        return true;
    }

    private async Task<MovieResponse> MapToResponseAsync(Movie movie)
    {
        var reviews = await _reviewRepository.GetByMovieIdAsync(movie.Id);
        var totalReviews = reviews.Count();
        var avgRating = totalReviews > 0 ? reviews.Average(r => r.Rating) : (decimal?)null;

        return new MovieResponse
        {
            Id = movie.Id,
            Name = movie.Name,
            Poster = movie.Poster,
            Description = movie.Description,
            ReleaseYear = movie.ReleaseYear,
            Type = movie.Type.ToString(),
            Category = JsonSerializer.Deserialize<List<string>>(movie.Category) ?? new List<string>(),
            Genre = JsonSerializer.Deserialize<List<string>>(movie.Genre) ?? new List<string>(),
            AvgRating = avgRating,
            TotalReviews = totalReviews,
            CreatedAt = movie.CreatedAt
        };
    }
}