using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Features.Expenses.Commands;

public class CreateExpenseCommand
{
    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public int CategoryId { get; set; }

    //public string Currency { get; set; } = "USD";

    public string? Notes { get; set; }

    public string? Merchant { get; set; }

    public string? PaymentMethod { get; set; }

    public ExpenseType ExpenseType { get; set; } = ExpenseType.Essential;

    public bool IsRecurring { get; set; }
}