using ExpenseTracker.WebApi.Contracts.Auth;
using FluentValidation;

namespace ExpenseTracker.WebApi.Validators.Auth;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password)
            .WithMessage("ConfirmPassword must match Password.");

        RuleFor(x => x.FirstName)
        .NotEmpty()
        .MaximumLength(100);


        RuleFor(x => x.LastName)
        .NotEmpty()
        .MaximumLength(100);
    }
}