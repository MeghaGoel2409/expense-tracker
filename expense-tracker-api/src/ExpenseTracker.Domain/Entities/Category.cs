using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class Category : BaseAuditableEntity
{
    private readonly List<Expense> _expenses = new();

    private Category()
    {
    }

    public Category(
        string name,
        string userId,
        string? description = null,
        string? icon = null,
        bool isSystemDefined = false)
    {
        SetName(name);

        UserId = userId;
        Description = description;
        Icon = icon;
        IsSystemDefined = isSystemDefined;
    }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? Icon { get; private set; }
    public bool IsSystemDefined { get; private set; }
    public string UserId { get; private set; } = default!;

    public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

    public static Category Create(
        string name,
        string userId,
        string? description = null,
        string? icon = null)
    {
        return new Category(name, userId, description, icon, false);
    }

    public static Category CreateSystem(
        string name,
        string? description = null,
        string? icon = null)
    {
        return new Category(name, "SYSTEM", description, icon, true);
    }

    public void UpdateDetails(string name, string? description, string? icon)
    {
        SetName(name);
        Description = description;
        Icon = icon;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters.");

        Name = name.Trim();
    }
}