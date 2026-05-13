using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Categories.Interfaces;
using ExpenseTracker.Application.Features.Expenses.DTOs;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Expenses.Commands;

public class UpdateExpenseCommandHandler
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateExpenseCommand> _validator;
    private readonly ILogger<UpdateExpenseCommandHandler> _logger;

    public UpdateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IValidator<UpdateExpenseCommand> validator,
        ILogger<UpdateExpenseCommandHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<ExpenseDto>> Handle(
        UpdateExpenseCommand request,
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

            return Result<ExpenseDto>.Failure(errors);
        }

        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return Result<ExpenseDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "User is not authenticated."));
        }

        var userId = _currentUserService.UserId;

        var expense = await _expenseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (expense == null || expense.IsDeleted)
        {
            return Result<ExpenseDto>.Failure(
                Error.NotFound("Expense.NotFound", "Expense not found."));
        }

        if (expense.UserId != userId)
        {
            _logger.LogWarning(
                "User {UserId} attempted to update expense {ExpenseId} owned by another user.",
                userId,
                request.Id);

            return Result<ExpenseDto>.Failure(
                Error.Forbidden("Expense.Forbidden", "You are not allowed to update this expense."));
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category == null || category.IsDeleted)
        {
            return Result<ExpenseDto>.Failure(
                Error.NotFound("Category.NotFound", "Category not found."));
        }

        if (!category.IsSystemDefined && category.UserId != userId)
        {
            _logger.LogWarning(
                "User {UserId} attempted to update expense {ExpenseId} with category {CategoryId} owned by another user.",
                userId,
                request.Id,
                request.CategoryId);

            return Result<ExpenseDto>.Failure(
                Error.Forbidden("Category.Forbidden", "You are not allowed to use this category."));
        }

        expense.Update(
            request.Amount,
            request.ExpenseDate,
            request.CategoryId,
            request.Currency,
            request.Notes,
            request.Merchant,
            request.PaymentMethod,
            request.ExpenseType,
            request.IsRecurring);

        expense.MarkAsModified(userId);

        _expenseRepository.Update(expense);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Expense {ExpenseId} updated by user {UserId}.",
            expense.Id,
            userId);

        var expenseDto = new ExpenseDto
        {
            Id = expense.Id,
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            Currency = expense.Currency,
            Notes = expense.Notes,
            Merchant = expense.Merchant,
            PaymentMethod = expense.PaymentMethod,
            CategoryId = expense.CategoryId,
            CategoryName = category.Name
        };

        return Result<ExpenseDto>.Success(expenseDto);
    }
}
