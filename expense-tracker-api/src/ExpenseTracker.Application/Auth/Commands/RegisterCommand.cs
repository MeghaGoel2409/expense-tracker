namespace ExpenseTracker.Application.Auth.Commands;

public sealed class RegisterCommand
{
    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public string? DeviceType { get; init; }
}