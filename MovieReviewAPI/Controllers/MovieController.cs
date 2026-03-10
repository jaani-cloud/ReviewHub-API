using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReview.Core.DTOs.Movie;
using MovieReview.Core.Interfaces.Services;
using MovieReviewAPI.Middleware;

namespace MovieReviewAPI.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieResponse>>> GetAllMovies()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                return Ok(new { success = true, data = movies });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieResponse>> GetMovieById(int id)
        {
            try
            {
                var movie = await _movieService.GetMovieByIdAsync(id);
                return Ok(new { success = true, data = movie });
            }
            catch (Exception ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MovieResponse>>> SearchMovies([FromQuery] string query)
        {
            try
            {
                var movies = await _movieService.SearchMoviesAsync(query);
                return Ok(new { success = true, data = movies });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [AdminOnly]
        public async Task<ActionResult<MovieResponse>> CreateMovie([FromBody] CreateMovieRequest request)
        {
            try
            {
                var movie = await _movieService.CreateMovieAsync(request);
                return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id },
                    new { success = true, message = "Movie created successfully", data = movie });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [AdminOnly]
        public async Task<ActionResult<MovieResponse>> UpdateMovie(int id, [FromBody] UpdateMovieRequest request)
        {
            try
            {
                var movie = await _movieService.UpdateMovieAsync(id, request);
                return Ok(new { success = true, message = "Movie updated successfully", data = movie });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [AdminOnly]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                await _movieService.DeleteMovieAsync(id);
                return Ok(new { success = true, message = "Movie deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}