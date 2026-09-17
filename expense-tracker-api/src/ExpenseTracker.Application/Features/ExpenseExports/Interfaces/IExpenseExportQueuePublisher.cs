using ExpenseTracker.Application.Features.ExpenseExports.DTOs;

namespace ExpenseTracker.Application.Features.ExpenseExports.Interfaces;

public interface IExpenseExportQueuePublisher
{
    Task PublishAsync(
        ExpenseExportQueueMessage message,
        CancellationToken cancellationToken = default);
}