using ExpenseTracker.Application.Features.ExpenseExports.DTOs;
using FluentValidation;

namespace ExpenseTracker.Application.Features.ExpenseExports.Validators;

public sealed class ExpenseExportFilterDtoValidator
    : AbstractValidator<ExpenseExportFilterDto>
{
    public ExpenseExportFilterDtoValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue)
            .WithMessage("Category ID must be greater than zero.");

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("To date must be on or after from date.");
    }
}