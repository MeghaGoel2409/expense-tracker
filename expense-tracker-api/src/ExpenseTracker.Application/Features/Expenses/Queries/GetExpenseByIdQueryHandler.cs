using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Expenses.DTOs;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Application.Features.Expenses.Models;
using FluentValidation;

namespace ExpenseTracker.Application.Features.Expenses.Queries;

public class GetExpenseByIdQueryHandler
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetExpenseByIdQueryHandler(
        IExpenseRepository expenseRepository,
        ICurrentUserService currentUserService)
    {
        _expenseRepository = expenseRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ExpenseDto>> Handle(
        GetExpenseByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return Error.Unauthorized("Auth.Unauthorized", "User is not authenticated.");
        }

        var expense = await _expenseRepository.GetByIdAsync(query.Id, cancellationToken);
        if (expense == null)
        {
            return Error.NotFound("Expense.NotFound", "Expense not found");
        }

        var dto = new ExpenseDto
        {
            Id = expense.Id,
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            Currency = expense.Currency,
            Notes = expense.Notes,
            Merchant = expense.Merchant,
            PaymentMethod = expense.PaymentMethod,
            CategoryId = expense.CategoryId,
            CategoryName = expense.Category?.Name ?? string.Empty,
            ExpenseType = expense.ExpenseType,
            IsRecurring = expense.IsRecurring
        };

        return Result<ExpenseDto>.Success(dto);
    }
}