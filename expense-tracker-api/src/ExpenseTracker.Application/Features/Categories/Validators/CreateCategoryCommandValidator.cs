using ExpenseTracker.Application.Features.Categories.Commands;
using FluentValidation;

namespace ExpenseTracker.Application.Features.Categories.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(300);

        RuleFor(x => x.Icon)
            .MaximumLength(100);
    }
}