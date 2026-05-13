namespace ExpenseTracker.Application.Features.Expenses.Queries;

public class GetExpensesQuery
{
    public int? CategoryId { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? SortBy { get; set; } = "date";

    public bool SortDescending { get; set; } = true;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}