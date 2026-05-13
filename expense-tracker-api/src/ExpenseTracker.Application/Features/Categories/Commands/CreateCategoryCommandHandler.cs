using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Categories.DTOs;
using ExpenseTracker.Application.Features.Categories.Interfaces;
using ExpenseTracker.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Application.Features.Categories.Commands;

public class CreateCategoryCommandHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IValidator<CreateCategoryCommand> validator,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<CategoryDto>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => (Error)ValidationError.Create(e.PropertyName, e.ErrorMessage, e.ErrorCode))
                .ToList();

            return Result<CategoryDto>.Failure(errors);
        }

        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return Result<CategoryDto>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "User is not authenticated."));
        }

        var userId = _currentUserService.UserId;

        var exists = await _categoryRepository.ExistsByNameAsync(userId, request.Name, cancellationToken);

        if (exists)
        {
            _logger.LogInformation(
                "User {UserId} attempted to create duplicate category with name {CategoryName}.",
                userId,
                request.Name);

            return Result<CategoryDto>.Failure(
                Error.Conflict("Category.Duplicate", "Category with same name already exists."));
        }

        var category = Category.Create(
            request.Name,
            userId,
            request.Description,
            request.Icon);

        category.MarkAsCreated(userId);

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Category {CategoryId} created by user {UserId} with name {CategoryName}.",
            category.Id,
            userId,
            category.Name);

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            IsSystemDefined = category.IsSystemDefined
        };

        return Result<CategoryDto>.Success(dto);
    }
}
