using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Expenses.Commands;
using ExpenseTracker.Application.Features.Expenses.DTOs;
using ExpenseTracker.Application.Features.Expenses.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Features.Expenses.Interfaces;
public interface IExpenseService
{
    Task<Result<PagedResult<ExpenseDto>>> GetExpensesAsync(GetExpensesQuery query, CancellationToken ct);

    Task<Result<ExpenseDto>> GetExpenseByIdAsync(GetExpenseByIdQuery query, CancellationToken ct);

    Task<Result<ExpenseDto>> CreateAsync(CreateExpenseCommand command, CancellationToken ct);

    Task<Result<ExpenseDto>> UpdateAsync(UpdateExpenseCommand command, CancellationToken ct);

    Task<Result> DeleteAsync(DeleteExpenseCommand command, CancellationToken ct);
}
