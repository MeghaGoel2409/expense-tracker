namespace ExpenseTracker.Application.Auth.Commands;

public sealed class LoginCommand
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public string? DeviceType { get; init; } // browser / mobile
}