using MovieReview.Core.DTOs.Review;
using MovieReview.Core.Interfaces.Repositories;
using MovieReview.Core.Interfaces.Services;
using MovieReview.Domain.Entities;
using MovieReview.Domain.Enums;

namespace MovieReview.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IUserRepository _userRepository;

    public ReviewService(
        IReviewRepository reviewRepository,
        IMovieRepository movieRepository,
        IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<ReviewResponse> CreateReviewAsync(int userId, CreateReviewRequest request)
    {
        // Check if movie exists
        var movie = await _movieRepository.GetByIdAsync(request.MovieId);
        if (movie == null)
            throw new InvalidOperationException("Movie not found");

        // Check if user already reviewed this movie
        var existingReview = await _reviewRepository.GetUserReviewForMovieAsync(userId, request.MovieId);
        if (existingReview != null)
            throw new InvalidOperationException("You have already reviewed this movie");

        // Validate review type
        if (!Enum.TryParse<ReviewType>(request.Type, true, out var reviewType))
            throw new ArgumentException("Invalid review type. Must be 'Skip', 'TimePass', or 'GoForIt'");

        // Calculate rating based on type
        decimal rating = reviewType switch
        {
            ReviewType.Skip => -0.5m,
            ReviewType.TimePass => 1.5m,
            ReviewType.GoForIt => 3m,
            _ => 0m
        };

        var review = new Domain.Entities.Review
        {
            MovieId = request.MovieId,
            UserId = userId,
            Type = reviewType,
            Comment = request.Comment,
            Rating = rating,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdReview = await _reviewRepository.AddAsync(review);
        return await MapToResponseAsync(createdReview);
    }

    public async Task<ReviewResponse> UpdateReviewAsync(int userId, int reviewId, UpdateReviewRequest request)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            throw new InvalidOperationException("Review not found");

        // Check ownership
        if (review.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own reviews");

        // Validate review type
        if (!Enum.TryParse<ReviewType>(request.Type, true, out var reviewType))
            throw new ArgumentException("Invalid review type. Must be 'Skip', 'TimePass', or 'GoForIt'");

        // Calculate rating
        decimal rating = reviewType switch
        {
            ReviewType.Skip => -0.5m,
            ReviewType.TimePass => 1.5m,
            ReviewType.GoForIt => 3m,
            _ => 0m
        };

        review.Type = reviewType;
        review.Comment = request.Comment;
        review.Rating = rating;
        review.UpdatedAt = DateTime.UtcNow;

        await _reviewRepository.UpdateAsync(review);
        return await MapToResponseAsync(review);
    }

    public async Task<bool> DeleteReviewAsync(int userId, int reviewId, bool isAdmin)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            throw new InvalidOperationException("Review not found");

        // Check ownership (admin can delete any review)
        if (!isAdmin && review.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own reviews");

        await _reviewRepository.DeleteAsync(reviewId);
        return true;
    }

    public async Task<IEnumerable<ReviewResponse>> GetReviewsByMovieIdAsync(int movieId)
    {
        var reviews = await _reviewRepository.GetByMovieIdAsync(movieId);
        var responses = new List<ReviewResponse>();

        foreach (var review in reviews)
        {
            responses.Add(await MapToResponseAsync(review));
        }

        return responses;
    }

    public async Task<IEnumerable<ReviewResponse>> GetReviewsByUserIdAsync(int userId)
    {
        var reviews = await _reviewRepository.GetByUserIdAsync(userId);
        var responses = new List<ReviewResponse>();

        foreach (var review in reviews)
        {
            responses.Add(await MapToResponseAsync(review));
        }

        return responses;
    }

    public async Task<ReviewResponse?> GetUserReviewForMovieAsync(int userId, int movieId)
    {
        var review = await _reviewRepository.GetUserReviewForMovieAsync(userId, movieId);
        if (review == null)
            return null;

        return await MapToResponseAsync(review);
    }

    private async Task<ReviewResponse> MapToResponseAsync(Domain.Entities.Review review)
    {
        var user = await _userRepository.GetByIdAsync(review.UserId);
        var movie = await _movieRepository.GetByIdAsync(review.MovieId);

        return new ReviewResponse
        {
            Id = review.Id,
            MovieId = review.MovieId,
            MovieName = movie?.Name ?? "Unknown",
            UserId = review.UserId,
            UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
            UserPhoto = user?.ProfilePhoto,
            Type = review.Type.ToString(),
            Comment = review.Comment,
            Rating = review.Rating,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}