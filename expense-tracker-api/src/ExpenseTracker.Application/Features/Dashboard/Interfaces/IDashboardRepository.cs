using ExpenseTracker.Application.Features.Dashboard.DTOs;

namespace ExpenseTracker.Application.Features.Dashboard.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(
        string userId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
}