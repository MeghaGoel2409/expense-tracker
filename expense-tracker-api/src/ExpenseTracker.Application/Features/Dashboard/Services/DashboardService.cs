using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Dashboard.DTOs;
using ExpenseTracker.Application.Features.Dashboard.Interfaces;
using ExpenseTracker.Application.Features.Dashboard.Queries;

namespace ExpenseTracker.Application.Features.Dashboard.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly GetDashboardSummaryQueryHandler _getDashboardSummaryHandler;

    public DashboardService(GetDashboardSummaryQueryHandler getDashboardSummaryHandler)
    {
        _getDashboardSummaryHandler = getDashboardSummaryHandler;
    }

    public Task<Result<DashboardSummaryDto>> GetSummaryAsync(
        GetDashboardSummaryQuery query,
        CancellationToken cancellationToken)
    {
        return _getDashboardSummaryHandler.HandleAsync(query, cancellationToken);
    }
}