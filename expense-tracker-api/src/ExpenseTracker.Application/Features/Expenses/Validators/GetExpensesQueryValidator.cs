using ExpenseTracker.Application.Features.Expenses.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Features.Expenses.Validators
{
    public class GetExpensesQueryValidator : AbstractValidator<GetExpensesQuery>
    {
        public GetExpensesQueryValidator()
        {
            RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than zero.");

            RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x)
           .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
           .WithMessage("FromDate cannot be greater than ToDate.");

            RuleFor(x => x.CategoryId)
                  .GreaterThan(0)
                  .When(x => x.CategoryId.HasValue)
                  .WithMessage("Category id must be greater than 0");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrWhiteSpace(x) ||
                    x.Equals("date", StringComparison.OrdinalIgnoreCase) ||
                    x.Equals("amount", StringComparison.OrdinalIgnoreCase))
                .WithMessage("SortBy must be either 'date' or 'amount'.");

        }
    }
}
