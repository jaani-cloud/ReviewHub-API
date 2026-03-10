using Microsoft.AspNetCore.Mvc;
using MovieReview.Core.DTOs.Auth;
using MovieReview.Core.Interfaces.Services;

namespace MovieReviewAPI.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _authService.RegisterAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.Password
            );

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Verification code sent to your email. Please verify within 10 minutes."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<AuthResponse>> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        try
        {
            await _authService.VerifyEmailAsync(request.Email, request.Code);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Email verified successfully! You can now login."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("resend-code")]
    public async Task<ActionResult<AuthResponse>> ResendCode([FromBody] ResendCodeRequest request)
    {
        try
        {
            await _authService.ResendVerificationCodeAsync(request.Email);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "New verification code sent to your email."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<AuthResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            await _authService.ForgotPasswordAsync(request.Email);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Password reset code sent to your email."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("verify-reset-code")]
    public async Task<ActionResult<AuthResponse>> VerifyResetCode([FromBody] VerifyResetCodeRequest request)
    {
        try
        {
            var resetToken = await _authService.VerifyResetCodeAsync(request.Email, request.Code);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Code verified. Use the reset token to set new password.",
                Token = resetToken
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<AuthResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            await _authService.ResetPasswordAsync(request.ResetToken, request.NewPassword);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Password reset successfully. You can now login with your new password."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}