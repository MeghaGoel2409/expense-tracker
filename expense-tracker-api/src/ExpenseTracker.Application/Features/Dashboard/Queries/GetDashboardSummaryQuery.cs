using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Dashboard.DTOs;

namespace ExpenseTracker.Application.Features.Dashboard.Queries;

public sealed class GetDashboardSummaryQuery
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}