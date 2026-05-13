namespace ExpenseTracker.Infrastructure.Authentication.Interfaces;

public interface IRefreshTokenGenerator
{
    string GenerateToken();
    string ComputeHash(string token);
}