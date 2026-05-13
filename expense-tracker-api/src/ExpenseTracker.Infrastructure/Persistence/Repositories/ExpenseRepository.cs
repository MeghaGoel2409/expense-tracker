using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Application.Features.Expenses.Models;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ExpenseRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Expense>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedData<Expense>> GetPagedByFilterAsync(
        ExpenseQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Expenses
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.UserId == filter.UserId && !x.IsDeleted)
            .AsQueryable();

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(x => x.ExpenseDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(x => x.ExpenseDate <= filter.ToDate.Value);
        }

        query = ApplySorting(query, filter.SortBy, filter.SortDescending);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedData<Expense>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        await _dbContext.Expenses.AddAsync(expense, cancellationToken);
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    public void Delete(Expense expense)
    {
        _dbContext.Expenses.Remove(expense);
    }

    private static IQueryable<Expense> ApplySorting(
        IQueryable<Expense> query,
        string? sortBy,
        bool sortDescending)
    {
        var normalizedSortBy = sortBy?.Trim().ToLowerInvariant();

        return normalizedSortBy switch
        {
            "amount" => sortDescending
                ? query.OrderByDescending(x => x.Amount)
                : query.OrderBy(x => x.Amount),

            _ => sortDescending
                ? query.OrderByDescending(x => x.ExpenseDate)
                : query.OrderBy(x => x.ExpenseDate)
        };
    }
}