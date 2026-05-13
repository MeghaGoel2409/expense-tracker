using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Expenses.DTOs;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Application.Features.Expenses.Models;
using FluentValidation;

namespace ExpenseTracker.Application.Features.Expenses.Queries;

public class GetExpensesQueryHandler
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<GetExpensesQuery> _validator;

    public GetExpensesQueryHandler(
        IExpenseRepository expenseRepository,
        ICurrentUserService currentUserService,
        IValidator<GetExpensesQuery> validator)
    {
        _expenseRepository = expenseRepository;
        _currentUserService = currentUserService;
        _validator = validator;
    }

    public async Task<Result<PagedResult<ExpenseDto>>> Handle(
        GetExpensesQuery request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => (Error)ValidationError.Create(
                    error.PropertyName,
                    error.ErrorMessage,
                    error.ErrorCode))
                .ToList();

            return Result<PagedResult<ExpenseDto>>.Failure(errors);
        }

        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return Result<PagedResult<ExpenseDto>>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "User is not authenticated."));
        }

        var filter = new ExpenseQueryFilter
        {
            UserId = _currentUserService.UserId,
            CategoryId = request.CategoryId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            SortBy = request.SortBy ?? "date",
            SortDescending = request.SortDescending,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var pagedExpenses = await _expenseRepository.GetPagedByFilterAsync(filter, cancellationToken);

        var items = pagedExpenses.Items
            .Select(expense => new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate,
                Currency = expense.Currency,
                Notes = expense.Notes,
                Merchant = expense.Merchant,
                PaymentMethod = expense.PaymentMethod,
                ExpenseType = expense.ExpenseType,
                CategoryId = expense.CategoryId,
                CategoryName = expense.Category?.Name ?? string.Empty,
                IsRecurring = expense.IsRecurring
            })
            .ToList();

        var result = new PagedResult<ExpenseDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = pagedExpenses.TotalCount
        };

        return Result<PagedResult<ExpenseDto>>.Success(result);
    }
}