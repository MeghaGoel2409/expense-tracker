using ExpenseTracker.Application.Auth.DTOs;

namespace ExpenseTracker.WebApi.Contracts.Auth;

public sealed class AuthTokenResponse
{
    public string AccessToken { get; init; } = default!;
    public DateTime ExpiresAtUtc { get; init; }

    public AuthUserDto User { get; init; } = default!;

}