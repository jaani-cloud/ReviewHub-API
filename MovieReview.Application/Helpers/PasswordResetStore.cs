using System.Collections.Concurrent;

namespace MovieReview.Application.Helpers;

public class PasswordResetStore
{
    private readonly ConcurrentDictionary<string, PendingPasswordReset> _store = new();

    public void Add(string email, PendingPasswordReset reset)
    {
        _store[email.ToLower()] = reset;
    }

    public PendingPasswordReset? Get(string email)
    {
        _store.TryGetValue(email.ToLower(), out var reset);
        return reset;
    }

    public PendingPasswordReset? GetByToken(string token)
    {
        return _store.Values.FirstOrDefault(x => x.ResetToken == token);
    }

    public bool Remove(string email)
    {
        return _store.TryRemove(email.ToLower(), out _);
    }

    public void CleanExpired()
    {
        var expired = _store.Where(x => x.Value.TokenExpiresAt < DateTime.UtcNow)
                           .Select(x => x.Key)
                           .ToList();

        foreach (var key in expired)
        {
            _store.TryRemove(key, out _);
        }
    }
}