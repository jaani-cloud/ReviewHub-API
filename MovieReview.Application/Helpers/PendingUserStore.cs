using System.Collections.Concurrent;

namespace MovieReview.Application.Helpers;

public class PendingUserStore
{
    private readonly ConcurrentDictionary<string, PendingUser> _store = new();

    public void Add(string email, PendingUser user)
    {
        _store[email.ToLower()] = user;
    }

    public PendingUser? Get(string email)
    {
        _store.TryGetValue(email.ToLower(), out var user);
        return user;
    }

    public bool Remove(string email)
    {
        return _store.TryRemove(email.ToLower(), out _);
    }

    public void CleanExpired()
    {
        var expired = _store.Where(x => x.Value.ExpiresAt < DateTime.UtcNow)
                           .Select(x => x.Key)
                           .ToList();

        foreach (var key in expired)
        {
            _store.TryRemove(key, out _);
        }
    }
}