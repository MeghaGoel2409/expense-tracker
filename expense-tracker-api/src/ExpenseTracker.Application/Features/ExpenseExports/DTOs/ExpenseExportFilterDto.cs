namespace ExpenseTracker.Application.Features.ExpenseExports.DTOs;

public sealed record ExpenseExportFilterDto
{
    public DateOnly? FromDate { get; init; }

    public DateOnly? ToDate { get; init; }

    public int? CategoryId { get; init; }

}