namespace ExpenseTracker.Application.Features.ExpenseExports.DTOs;

public sealed record ExpenseExportQueueMessage(
    int ExportJobId,
    string UserId,
    string CorrelationId);