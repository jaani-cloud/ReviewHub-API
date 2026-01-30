using BCrypt.Net;
using MovieReview.Application.Helpers;
using MovieReview.Core.Interfaces.Repositories;
using MovieReview.Core.Interfaces.Services;
using MovieReview.Domain.Entities;

namespace MovieReview.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly PendingUserStore _pendingUserStore;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IEmailService emailService,
        PendingUserStore pendingUserStore)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _pendingUserStore = pendingUserStore ?? throw new ArgumentNullException(nameof(pendingUserStore));
    }

    public async Task<bool> RegisterAsync(string firstName, string lastName,
        string email, string phoneNumber, string password)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required", nameof(phoneNumber));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required", nameof(password));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        if (!IsValidPhoneNumber(phoneNumber))
            throw new ArgumentException("Phone number must be exactly 10 digits", nameof(phoneNumber));

        _pendingUserStore.CleanExpired();

        if (await _userRepository.EmailExistsAsync(email))
            throw new InvalidOperationException("Email already registered");

        var existing = _pendingUserStore.Get(email);
        if (existing != null && existing.ExpiresAt > DateTime.UtcNow)
            throw new InvalidOperationException("Verification code already sent. Please check your email or use resend.");

        var code = new Random().Next(100000, 999999).ToString();

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var pendingUser = new PendingUser
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            HashedPassword = hashedPassword,
            VerificationCode = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        _pendingUserStore.Add(email, pendingUser);

        try
        {
            await _emailService.SendVerificationEmailAsync(email, code, firstName);
        }
        catch (Exception ex)
        {
            _pendingUserStore.Remove(email);
            throw new InvalidOperationException($"Failed to send verification email: {ex.Message}");
        }

        return true;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required", nameof(password));

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!user.EmailVerified)
            throw new UnauthorizedAccessException("Email not verified. Please verify your email first.");

        return _jwtService.GenerateToken(user);
    }

    public async Task<bool> VerifyEmailAsync(string email, string code)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Verification code is required", nameof(code));

        var pendingUser = _pendingUserStore.Get(email);
        if (pendingUser == null)
            throw new InvalidOperationException("No pending registration found for this email");

        if (pendingUser.ExpiresAt < DateTime.UtcNow)
        {
            _pendingUserStore.Remove(email);
            throw new InvalidOperationException("Verification code expired. Please register again.");
        }

        if (pendingUser.VerificationCode != code)
            throw new InvalidOperationException("Invalid verification code");

        var user = new User
        {
            FirstName = pendingUser.FirstName,
            LastName = pendingUser.LastName,
            Email = pendingUser.Email,
            PhoneNumber = pendingUser.PhoneNumber,
            Password = pendingUser.HashedPassword,
            EmailVerified = true, 
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        _pendingUserStore.Remove(email);

        return true;
    }

    public async Task<bool> ResendVerificationCodeAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        var pendingUser = _pendingUserStore.Get(email);
        if (pendingUser == null)
            throw new InvalidOperationException("No pending registration found for this email");

        var code = new Random().Next(100000, 999999).ToString();

        pendingUser.VerificationCode = code;
        pendingUser.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
        _pendingUserStore.Add(email, pendingUser);

        await _emailService.SendVerificationEmailAsync(email, code, pendingUser.FirstName);

        return true;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[0-9]{10}$");
    }
}