using ExpenseTracker.Application.Features.ExpenseExports.DTOs;

namespace ExpenseTracker.Application.Features.ExpenseExports.Interfaces;

public interface IExpenseExportProcessor
{
    Task ProcessAsync(
        ExpenseExportQueueMessage message,
        CancellationToken cancellationToken = default);
}