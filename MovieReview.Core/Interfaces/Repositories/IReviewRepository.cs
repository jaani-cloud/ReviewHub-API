using MovieReview.Domain.Entities;

namespace MovieReview.Core.Interfaces.Repositories;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetByMovieIdAsync(int movieId);
    Task<IEnumerable<Review>> GetByUserIdAsync(int userId);
    Task<Review?> GetUserReviewForMovieAsync(int userId, int movieId);
}