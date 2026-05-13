using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Dashboard.DTOs;
using ExpenseTracker.Application.Features.Dashboard.Interfaces;
using ExpenseTracker.Application.Features.Expenses.DTOs;
using FluentValidation;

namespace ExpenseTracker.Application.Features.Dashboard.Queries;

public sealed class GetDashboardSummaryQueryHandler
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<GetDashboardSummaryQuery> _validator;

    public GetDashboardSummaryQueryHandler(
        IDashboardRepository dashboardRepository,
        ICurrentUserService currentUserService,
        IValidator<GetDashboardSummaryQuery> validator)
    {
        _dashboardRepository = dashboardRepository;
        _currentUserService = currentUserService;
        _validator = validator;
    }

    public async Task<Result<DashboardSummaryDto>> HandleAsync(
        GetDashboardSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<DashboardSummaryDto>.Failure(
                validationResult.Errors
                    .Select(x => ValidationError.Create(x.PropertyName, x.ErrorMessage, x.ErrorCode))
                    .ToList());
        }

        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return Result<DashboardSummaryDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "User is not authenticated."));
        }

        var today = DateTime.UtcNow.Date;

        var fromDate = query.FromDate?.Date
            ?? new DateTime(today.Year, today.Month, 1);

        var toDate = query.ToDate?.Date
            ?? today;

        var dashboard = await _dashboardRepository.GetSummaryAsync(
            _currentUserService.UserId,
            fromDate,
            toDate,
            cancellationToken);

        return Result<DashboardSummaryDto>.Success(dashboard);
    }
}