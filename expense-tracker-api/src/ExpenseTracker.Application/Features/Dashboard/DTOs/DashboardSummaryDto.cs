namespace ExpenseTracker.Application.Features.Dashboard.DTOs;

public sealed class DashboardSummaryDto
{
    public string Currency { get; set; } = "USD";

    public decimal TotalExpenses { get; set; }
    public int ExpenseCount { get; set; }
    public decimal EssentialTotal { get; set; }
    public decimal NonEssentialTotal { get; set; }
    public decimal RecurringTotal { get; set; }

    public string? TopCategory { get; set; }

    public List<CategoryExpenseSummaryDto> CategoryBreakdown { get; set; } = [];
}