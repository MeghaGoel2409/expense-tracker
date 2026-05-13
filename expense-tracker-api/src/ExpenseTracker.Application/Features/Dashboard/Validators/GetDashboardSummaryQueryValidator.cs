using ExpenseTracker.Application.Features.Dashboard.Queries;
using FluentValidation;

namespace ExpenseTracker.Application.Features.Dashboard.Validators;

public sealed class GetDashboardSummaryQueryValidator
    : AbstractValidator<GetDashboardSummaryQuery>
{
    public GetDashboardSummaryQueryValidator()
    {
        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
            .WithMessage("FromDate must be less than or equal to ToDate.");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .When(x => x.FromDate.HasValue)
            .WithMessage("FromDate cannot be in the future.");
    }
}