using Microsoft.EntityFrameworkCore;
using MovieReview.Core.Interfaces.Repositories;
using MovieReview.Domain.Entities;
using MovieReview.Infrastructure.Data;

namespace MovieReview.Infrastructure.Repositories;

public class MovieRepository : Repository<Movie>, IMovieRepository
{
    public MovieRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Movie>> SearchMoviesAsync(string search)
    {
        return await _dbSet
            .Where(m => m.Name.Contains(search) || m.Description.Contains(search))
            .ToListAsync();
    }
}