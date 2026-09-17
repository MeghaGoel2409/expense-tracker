using ExpenseTracker.Application.Features.ExpenseExports.DTOs;
using FluentValidation;

namespace ExpenseTracker.Application.Features.ExpenseExports.Validators;

public sealed class CreateExpenseExportRequestValidator
    : AbstractValidator<CreateExpenseExportRequest>
{
    public CreateExpenseExportRequestValidator(
        IValidator<ExpenseExportFilterDto> filterValidator)
    {
        RuleFor(x => x.Format)
            .IsInEnum()
            .WithMessage("A valid export format is required.");

        RuleFor(x => x.Filter)
            .NotNull()
            .WithMessage("Export filters are required.")
            .SetValidator(filterValidator);
    }
}