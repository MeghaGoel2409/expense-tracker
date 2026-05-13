using ExpenseTracker.WebApi.Contracts.Auth;
using FluentValidation;

namespace ExpenseTracker.WebApi.Validators.Auth;

public sealed class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();

    }
}