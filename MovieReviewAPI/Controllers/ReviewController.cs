using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReview.Core.DTOs.Review;
using MovieReview.Core.Interfaces.Services;
using MovieReviewAPI.Middleware;
using System.Security.Claims;

namespace MovieReviewAPI.Controllers;

[Route("api/review")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            throw new UnauthorizedAccessException("Invalid token");

        return userId;
    }

    private bool IsAdmin()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        return roleClaim == "Admin";
    }

    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetReviewsByMovie(int movieId)
    {
        try
        {
            var reviews = await _reviewService.GetReviewsByMovieIdAsync(movieId);
            return Ok(new { success = true, data = reviews });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetReviewsByUser(int userId)
    {
        try
        {
            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
            return Ok(new { success = true, data = reviews });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("user/{userId}/movie/{movieId}")]
    public async Task<ActionResult<ReviewResponse>> GetUserReviewForMovie(int userId, int movieId)
    {
        try
        {
            var review = await _reviewService.GetUserReviewForMovieAsync(userId, movieId);
            if (review == null)
                return NotFound(new { success = false, message = "Review not found" });

            return Ok(new { success = true, data = review });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewResponse>> CreateReview([FromBody] CreateReviewRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var review = await _reviewService.CreateReviewAsync(userId, request);
            return CreatedAtAction(nameof(GetUserReviewForMovie),
                new { userId = review.UserId, movieId = review.MovieId },
                new { success = true, message = "Review created successfully", data = review });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ReviewResponse>> UpdateReview(int id, [FromBody] UpdateReviewRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var review = await _reviewService.UpdateReviewAsync(userId, id, request);
            return Ok(new { success = true, message = "Review updated successfully", data = review });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var isAdmin = IsAdmin();
            await _reviewService.DeleteReviewAsync(userId, id, isAdmin);
            return Ok(new { success = true, message = "Review deleted successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}