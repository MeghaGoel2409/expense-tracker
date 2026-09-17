using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Features.ExpenseExports.DTOs;

public sealed record CreateExpenseExportRequest
{
    public ExportFormat Format { get; init; } = ExportFormat.Csv;

    public ExpenseExportFilterDto Filter { get; init; } = new();
}