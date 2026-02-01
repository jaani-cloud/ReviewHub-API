using MovieReview.Core.DTOs.Review;

namespace MovieReview.Core.Interfaces.Services;

public interface IReviewService
{
    Task<ReviewResponse> CreateReviewAsync(int userId, CreateReviewRequest request);
    Task<ReviewResponse> UpdateReviewAsync(int userId, int reviewId, UpdateReviewRequest request);
    Task<bool> DeleteReviewAsync(int userId, int reviewId, bool isAdmin);
    Task<IEnumerable<ReviewResponse>> GetReviewsByMovieIdAsync(int movieId);
    Task<IEnumerable<ReviewResponse>> GetReviewsByUserIdAsync(int userId);
    Task<ReviewResponse?> GetUserReviewForMovieAsync(int userId, int movieId);
}