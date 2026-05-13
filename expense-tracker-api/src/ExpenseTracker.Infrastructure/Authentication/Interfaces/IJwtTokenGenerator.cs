namespace ExpenseTracker.Infrastructure.Authentication.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(string userId, string email, IEnumerable<string> roles);
    int AccessTokenExpirySeconds { get; }
}