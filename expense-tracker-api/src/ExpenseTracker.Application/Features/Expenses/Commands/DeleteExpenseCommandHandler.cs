using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Expenses.Commands;

public class DeleteExpenseCommandHandler
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteExpenseCommand> _validator;
    private readonly ILogger<DeleteExpenseCommandHandler> _logger;

    public DeleteExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IValidator<DeleteExpenseCommand> validator,
        ILogger<DeleteExpenseCommandHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result> Handle(
        DeleteExpenseCommand request,
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

            return Result.Failure(errors);
        }

        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return Result.Failure(
                Error.Unauthorized("Auth.Unauthorized", "User is not authenticated."));
        }

        var userId = _currentUserService.UserId;

        var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (expense == null || expense.IsDeleted)
        {
            return Result.Failure(
                Error.NotFound("Expense.NotFound", "Expense not found."));
        }

        if (expense.UserId != userId)
        {
            _logger.LogWarning(
                "User {UserId} attempted to delete expense {ExpenseId} owned by another user.",
                userId,
                request.Id);

            return Result.Failure(
                Error.Forbidden("Expense.Forbidden", "You are not allowed to delete this expense."));
        }

        expense.SoftDelete(userId);

        _expenseRepository.Update(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Expense {ExpenseId} soft deleted by user {UserId}.",
            expense.Id,
            userId);

        return Result.Success();
    }
}
