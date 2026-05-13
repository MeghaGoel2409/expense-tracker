using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Features.Categories.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Category>> GetAccessibleCategoriesAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string userId, string name, CancellationToken cancellationToken = default);

    Task AddAsync(Category category, CancellationToken cancellationToken = default);
}