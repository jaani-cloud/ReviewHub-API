using Microsoft.EntityFrameworkCore;
using MovieReview.Core.Interfaces.Repositories;
using MovieReview.Domain.Entities;
using MovieReview.Infrastructure.Data;

namespace MovieReview.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetByMovieIdAsync(int movieId)
        {
            return await _dbSet
                .Where(r => r.MovieId == movieId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .Include(r => r.Movie)
                .ToListAsync();
        }

        public async Task<Review?> GetUserReviewForMovieAsync(int userId, int movieId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
        }
    }
}