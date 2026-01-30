using MovieReview.Domain.Entities;

namespace MovieReview.Core.Interfaces.Repositories;

public interface IMovieRepository : IRepository<Movie>
{
    Task<IEnumerable<Movie>> SearchMoviesAsync(string search);
}