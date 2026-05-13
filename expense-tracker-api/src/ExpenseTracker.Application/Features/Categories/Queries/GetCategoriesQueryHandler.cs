using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Categories.DTOs;
using ExpenseTracker.Application.Features.Categories.Interfaces;

namespace ExpenseTracker.Application.Features.Categories.Queries;

public class GetCategoriesQueryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCategoriesQueryHandler(
        ICategoryRepository categoryRepository,
        ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            return Result<IReadOnlyList<CategoryDto>>.Failure(
                Error.Unauthorized("Auth.Unauthorized", "User is not authenticated."));
        }

        var categories = await _categoryRepository.GetAccessibleCategoriesAsync(
            _currentUserService.UserId,
            cancellationToken);

        var items = categories
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Icon = category.Icon,
                IsSystemDefined = category.IsSystemDefined
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<CategoryDto>>.Success(items);
    }
}