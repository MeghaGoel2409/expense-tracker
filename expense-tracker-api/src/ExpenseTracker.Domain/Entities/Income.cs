using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class Income : BaseAuditableEntity
{
    private Income()
    {
    }

    public Income(
        decimal amount,
        DateTime receivedOn,
        string source,
        string userId,
        string currency = "USD",
        string? notes = null,
        bool isRecurring = false)
    {
        SetAmount(amount);
        SetSource(source);
        SetCurrency(currency);

        ReceivedOn = receivedOn;
        UserId = userId;
        Notes = notes?.Trim();
        IsRecurring = isRecurring;
    }

    public decimal Amount { get; private set; }
    public DateTime ReceivedOn { get; private set; }
    public string Source { get; private set; } = default!;
    public string Currency { get; private set; } = default!;
    public string? Notes { get; private set; }
    public bool IsRecurring { get; private set; }

    public string UserId { get; private set; } = default!;

    public void Update(
        decimal amount,
        DateTime receivedOn,
        string source,
        string currency,
        string? notes,
        bool isRecurring)
    {
        SetAmount(amount);
        SetSource(source);
        SetCurrency(currency);

        ReceivedOn = receivedOn;
        Notes = notes?.Trim();
        IsRecurring = isRecurring;
    }

    private void SetAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Income amount must be greater than zero.");

        Amount = amount;
    }

    private void SetSource(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("Income source is required.");

        if (source.Length > 100)
            throw new ArgumentException("Income source cannot exceed 100 characters.");

        Source = source.Trim();
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