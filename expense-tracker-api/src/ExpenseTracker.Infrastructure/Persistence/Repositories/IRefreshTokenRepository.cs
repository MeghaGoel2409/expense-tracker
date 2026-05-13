using ExpenseTracker.Infrastructure.Persistence.Entities;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task RevokeAllActiveTokensAsync(string userId, string? ip, CancellationToken cancellationToken);
}