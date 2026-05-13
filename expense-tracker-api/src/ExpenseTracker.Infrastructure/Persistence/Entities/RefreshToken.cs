namespace ExpenseTracker.Infrastructure.Persistence.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }

    public string UserId { get; private set; } = default!;

    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public string? CreatedByIp { get; private set; }
    public string? RevokedByIp { get; private set; }

    public string? DeviceType { get; private set; }
    public string? UserAgent { get; private set; }

    private RefreshToken() { }

    public RefreshToken(
        string userId,
        string tokenHash,
        DateTime expiresAtUtc,
        DateTime createdAtUtc,
        string? createdByIp,
        string? deviceType,
        string? userAgent)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = createdAtUtc;
        CreatedByIp = createdByIp;
        DeviceType = deviceType;
        UserAgent = userAgent;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    public void Revoke(DateTime revokedAtUtc, string? revokedByIp, string? replacedByTokenHash = null)
    {
        if (IsRevoked)
        {
            return;
        }

        RevokedAtUtc = revokedAtUtc;
        RevokedByIp = revokedByIp;
        ReplacedByTokenHash = replacedByTokenHash;
    }
}