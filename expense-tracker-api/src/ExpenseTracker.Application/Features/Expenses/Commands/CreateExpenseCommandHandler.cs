using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Categories.Interfaces;
using ExpenseTracker.Application.Features.Expenses.DTOs;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Expenses.Commands;

public class CreateExpenseCommandHandler
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateExpenseCommand> _validator;
    private readonly ILogger<CreateExpenseCommandHandler> _logger;

    public CreateExpenseCommandHandler(
        IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        IValidator<CreateExpenseCommand> validator,
        ILogger<CreateExpenseCommandHandler> logger)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<ExpenseDto>> Handle(
        CreateExpenseCommand request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(error =>
                (Error)ValidationError.Create(error.PropertyName, error.ErrorMessage, error.ErrorCode)).ToList();
            return Result<ExpenseDto>.Failure(errors);
        }

        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return Result<ExpenseDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "User is not authenticated."));
        }

        var userId = _currentUserService.UserId;

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category == null || category.IsDeleted)
        {
            return Result<ExpenseDto>.Failure(
                Error.NotFound("Category.NotFound", "Category not found."));
        }

        if (!category.IsSystemDefined && category.UserId != userId)
        {
            _logger.LogWarning(
                "User {UserId} attempted to create an expense with category {CategoryId} owned by another user.",
                userId,
                request.CategoryId);

            return Result<ExpenseDto>.Failure(
                Error.Forbidden("Category.Forbidden", "You are not allowed to use this category."));
        }

        var currency = await _identityService.GetDefaultCurrencyAsync(
             userId,
            cancellationToken);

        var expense = new Expense(
            request.Amount,
            request.ExpenseDate,
            request.CategoryId,
            userId,
            currency,
            request.Notes,
            request.Merchant,
            request.PaymentMethod,
            request.ExpenseType,
            request.IsRecurring);

        expense.MarkAsCreated(userId);

        await _expenseRepository.AddAsync(expense, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Expense {ExpenseId} created by user {UserId} for category {CategoryId} with amount {Amount} {Currency}.",
            expense.Id,
            userId,
            expense.CategoryId,
            expense.Amount,
            expense.Currency);

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
