using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

public class Expense : BaseAuditableEntity
{
    private Expense()
    {
    }

    public Expense(
        decimal amount,
        DateTime expenseDate,
        int categoryId,
        string userId,
        string currency = "USD",
        string? notes = null,
        string? merchant = null,
        string? paymentMethod = null,
        ExpenseType expenseType = ExpenseType.Essential,
        bool isRecurring = false)
    {
        SetAmount(amount);
        SetCurrency(currency);

        ExpenseDate = expenseDate;
        CategoryId = categoryId;
        UserId = userId;
        Notes = notes?.Trim();
        Merchant = merchant?.Trim();
        PaymentMethod = paymentMethod?.Trim();
        ExpenseType = expenseType;
        IsRecurring = isRecurring;
    }

    public decimal Amount { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    public string Currency { get; set; }
    public string? Notes { get; private set; }
    public string? Merchant { get; private set; }
    public string? PaymentMethod { get; private set; }
    public ExpenseType ExpenseType { get; private set; }
    public bool IsRecurring { get; private set; }

    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;

    public string UserId { get; private set; } = default!;

    public void Update(
        decimal amount,
        DateTime expenseDate,
        int categoryId,
        string currency,
        string? notes,
        string? merchant,
        string? paymentMethod,
        ExpenseType expenseType,
        bool isRecurring)
    {
        SetAmount(amount);
        SetCurrency(currency);

        ExpenseDate = expenseDate;
        CategoryId = categoryId;
        Notes = notes?.Trim();
        Merchant = merchant?.Trim();
        PaymentMethod = paymentMethod?.Trim();
        ExpenseType = expenseType;
        IsRecurring = isRecurring;
    }

    private void SetAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Expense amount must be greater than zero.");

        Amount = amount;
    }

    private void SetCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.");

        if (currency.Length > 10)
            throw new ArgumentException("Currency cannot exceed 10 characters.");

        Currency = currency.Trim().ToUpper();
    }
}