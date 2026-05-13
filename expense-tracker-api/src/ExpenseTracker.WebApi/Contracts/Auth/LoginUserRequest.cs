namespace ExpenseTracker.WebApi.Contracts.Auth;

public sealed class LoginUserRequest
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}