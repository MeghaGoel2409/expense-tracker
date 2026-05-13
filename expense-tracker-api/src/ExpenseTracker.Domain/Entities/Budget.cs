using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

public class Budget : BaseAuditableEntity
{
    private Budget()
    {
    }

    public Budget(
        string name,
        decimal limitAmount,
        BudgetPeriod period,
        DateTime startDate,
        DateTime endDate,
        string userId,
        int? categoryId = null)
    {
        SetName(name);
        SetLimitAmount(limitAmount);

        if (endDate < startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");

        Period = period;
        StartDate = startDate;
        EndDate = endDate;
        UserId = userId;
        CategoryId = categoryId;
    }

    public string Name { get; private set; } = default!;
    public decimal LimitAmount { get; private set; }
    public BudgetPeriod Period { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public int? CategoryId { get; private set; }
    public Category? Category { get; private set; }

    public string UserId { get; private set; } = default!;

    public void Update(
        string name,
        decimal limitAmount,
        BudgetPeriod period,
        DateTime startDate,
        DateTime endDate,
        int? categoryId)
    {
        SetName(name);
        SetLimitAmount(limitAmount);

        if (endDate < startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");

        Period = period;
        StartDate = startDate;
        EndDate = endDate;
        CategoryId = categoryId;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Budget name is required.");

        if (name.Length > 100)
            throw new ArgumentException("Budget name cannot exceed 100 characters.");

        Name = name.Trim();
    }

    private void SetLimitAmount(decimal limitAmount)
    {
        if (limitAmount <= 0)
            throw new ArgumentException("Budget limit amount must be greater than zero.");

        LimitAmount = limitAmount;
    }
}