using ExpenseTracker.Application.Features.Dashboard.DTOs;
using ExpenseTracker.Application.Features.Dashboard.Interfaces;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public sealed class DashboardRepository : IDashboardRepository
{
    private const string FallbackCurrency = "USD";

    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(
        string userId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var currency = await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.DefaultCurrency)
            .FirstOrDefaultAsync(cancellationToken);

        currency = string.IsNullOrWhiteSpace(currency)
            ? FallbackCurrency
            : currency;

        var expensesQuery = _context.Expenses
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.ExpenseDate >= fromDate &&
                x.ExpenseDate <= toDate &&
                x.Currency == currency);

        var totalExpenses = await expensesQuery
            .SumAsync(x => x.Amount, cancellationToken);

        var expenseCount = await expensesQuery
            .CountAsync(cancellationToken);

        var essentialTotal = await expensesQuery
            .Where(x => x.ExpenseType == ExpenseType.Essential)
            .SumAsync(x => x.Amount, cancellationToken);

        var nonEssentialTotal = await expensesQuery
            .Where(x => x.ExpenseType == ExpenseType.NonEssential)
            .SumAsync(x => x.Amount, cancellationToken);

        var recurringTotal = await expensesQuery
            .Where(x => x.IsRecurring)
            .SumAsync(x => x.Amount, cancellationToken);

        var categoryBreakdown = await expensesQuery
            .GroupBy(x => new
            {
                x.CategoryId,
                CategoryName = x.Category.Name
            })
            .Select(g => new CategoryExpenseSummaryDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.CategoryName,
                TotalAmount = g.Sum(x => x.Amount),
                Currency = currency,
                TransactionCount = g.Count()
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync(cancellationToken);

        foreach (var category in categoryBreakdown)
        {
            category.Percentage = totalExpenses == 0
                ? 0
                : Math.Round((category.TotalAmount / totalExpenses) * 100, 2);
        }

        return new DashboardSummaryDto
        {
            Currency = currency,

            TotalExpenses = totalExpenses,
            ExpenseCount = expenseCount,            

            EssentialTotal = essentialTotal,
            NonEssentialTotal = nonEssentialTotal,
            RecurringTotal = recurringTotal,

            TopCategory = categoryBreakdown.FirstOrDefault()?.CategoryName,
            CategoryBreakdown = categoryBreakdown
        };
    }
}