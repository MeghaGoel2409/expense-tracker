namespace ExpenseTracker.Application.Auth.Commands;

public sealed class RefreshTokenCommand
{
    public string RefreshToken { get; init; } = default!;
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}