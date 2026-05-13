using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Expenses.Models;
using ExpenseTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Features.Expenses.Interfaces
{
    public interface IExpenseRepository
    {
        Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Expense>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<PagedData<Expense>> GetPagedByFilterAsync(
        ExpenseQueryFilter filter,
        CancellationToken cancellationToken = default);

        Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

        void Update(Expense expense);

        //Task Delete(Expense expense);
    }
}
