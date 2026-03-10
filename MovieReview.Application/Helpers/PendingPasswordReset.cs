namespace MovieReview.Application.Helpers;

public class PendingPasswordReset
{
    public string Email { get; set; } = string.Empty;
    public string ResetCode { get; set; } = string.Empty;
    public string ResetToken { get; set; } = string.Empty;
    public DateTime CodeExpiresAt { get; set; }
    public DateTime TokenExpiresAt { get; set; }
    public bool CodeVerified { get; set; }
}