namespace MovieReview.Domain.Entities;

public class EmailVerification
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool Verified { get; set; }
}