namespace ExpenseTracker.Application.Features.Categories.DTOs;

public class CategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public bool IsSystemDefined { get; set; }
}