namespace ExpenseTracker.Application.Features.Dashboard.DTOs;

public sealed class CategoryExpenseSummaryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "USD";

    public decimal Percentage { get; set; }
    public int TransactionCount { get; set; }
}