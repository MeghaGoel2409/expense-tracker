namespace ExpenseTracker.Application.Features.Categories.Commands;

public class CreateCategoryCommand
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public string? Icon { get; set; }
}