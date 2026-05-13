namespace ExpenseTracker.Application.Common.Models;

public class PagedData<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    public int TotalCount { get; init; }
}