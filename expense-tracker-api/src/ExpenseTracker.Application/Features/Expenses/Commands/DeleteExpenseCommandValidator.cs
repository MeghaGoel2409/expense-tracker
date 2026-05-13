using ExpenseTracker.Application.Features.Expenses.Commands;
using FluentValidation;

namespace ExpenseTracker.Application.Features.Expenses.Validators;

public class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
{
    public DeleteExpenseCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Expense Id must be greater than zero.");
    }
}