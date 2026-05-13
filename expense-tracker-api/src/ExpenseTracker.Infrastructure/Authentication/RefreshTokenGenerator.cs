namespace ExpenseTracker.Infrastructure.Authentication;

using ExpenseTracker.Infrastructure.Authentication.Interfaces;
using System.Security.Cryptography;
using System.Text;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToHexString(bytes);
    }

    public string ComputeHash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}