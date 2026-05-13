using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Expenses.Commands;
using ExpenseTracker.Application.Features.Expenses.DTOs;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Application.Features.Expenses.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Features.Expenses.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly GetExpensesQueryHandler _getExpensesHandler;
        private readonly GetExpenseByIdQueryHandler _getExpenseByIdHandler;
        private readonly CreateExpenseCommandHandler _createHandler;
        private readonly UpdateExpenseCommandHandler _updateHandler;
        private readonly DeleteExpenseCommandHandler _deleteHandler;

        public ExpenseService(
            GetExpensesQueryHandler getExpensesHandler,
            GetExpenseByIdQueryHandler getExpenseByIdHandler,
            CreateExpenseCommandHandler createHandler,
            UpdateExpenseCommandHandler updateHandler,
            DeleteExpenseCommandHandler deleteHandler)
        {
            _getExpensesHandler = getExpensesHandler;
            _getExpenseByIdHandler = getExpenseByIdHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
        }

        public Task<Result<PagedResult<ExpenseDto>>> GetExpensesAsync(GetExpensesQuery query, CancellationToken ct)
            => _getExpensesHandler.Handle(query, ct);

        public Task<Result<ExpenseDto>> GetExpenseByIdAsync(GetExpenseByIdQuery query, CancellationToken ct)
            => _getExpenseByIdHandler.Handle(query, ct);

        public Task<Result<ExpenseDto>> CreateAsync(CreateExpenseCommand command, CancellationToken ct)
            => _createHandler.Handle(command, ct);

        public Task<Result<ExpenseDto>> UpdateAsync(UpdateExpenseCommand command, CancellationToken ct)
            => _updateHandler.Handle(command, ct);

        public Task<Result> DeleteAsync(DeleteExpenseCommand command, CancellationToken ct)
            => _deleteHandler.Handle(command, ct);
    }
}
