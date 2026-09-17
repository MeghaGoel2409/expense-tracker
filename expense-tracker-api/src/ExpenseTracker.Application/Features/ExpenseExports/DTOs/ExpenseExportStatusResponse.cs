using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Features.ExpenseExports.DTOs;

public sealed record ExpenseExportStatusResponse(
    int ExportJobId,
    ExportJobStatus Status,
    string? ErrorMessage,
    DateTime CreatedOnUtc,
    DateTime? StartedOnUtc,
    DateTime? CompletedOnUtc);