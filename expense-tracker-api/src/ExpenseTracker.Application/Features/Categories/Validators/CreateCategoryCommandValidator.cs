using ExpenseTracker.Application.Features.Categories.Commands;
using FluentValidation;

namespace ExpenseTracker.Application.Features.Categories.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description cannot exceed 300 characters.");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("Icon cannot exceed 100 characters.");
    }
}