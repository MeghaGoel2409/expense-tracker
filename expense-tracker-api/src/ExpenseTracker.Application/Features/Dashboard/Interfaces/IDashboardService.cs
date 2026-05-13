using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Dashboard.DTOs;
using ExpenseTracker.Application.Features.Dashboard.Queries;

namespace ExpenseTracker.Application.Features.Dashboard.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardSummaryDto>> GetSummaryAsync(
        GetDashboardSummaryQuery query,
        CancellationToken cancellationToken);
}