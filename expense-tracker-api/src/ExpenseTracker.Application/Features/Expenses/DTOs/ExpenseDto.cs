using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Features.Expenses.DTOs;

public class ExpenseDto
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public string Currency { get; set; } = "USD";

    public string? Notes { get; set; }

    public string? Merchant { get; set; }

    public string? PaymentMethod { get; set; }

    public ExpenseType ExpenseType { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = default!;

    public bool IsRecurring { get; set; }
}