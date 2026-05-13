namespace ExpenseTracker.WebApi.Contracts.Auth;

public sealed class RegisterUserRequest
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string ConfirmPassword { get; init; } = default!;

    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;
}