using FluentValidation;
using ExpenseTracker.Application.Features.Expenses.Commands;

namespace ExpenseTracker.Application.Features.Expenses.Validators;

public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId must be greater than zero.");

        //RuleFor(x => x.Currency)
        //    .NotEmpty()
        //    .MaximumLength(10);

        RuleFor(x => x.ExpenseDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Expense date cannot be in the future.");

        RuleFor(x => x.Notes)
            .MaximumLength(500);

        RuleFor(x => x.Merchant)
            .MaximumLength(200);

        RuleFor(x => x.PaymentMethod)
            .MaximumLength(50);
    }
}