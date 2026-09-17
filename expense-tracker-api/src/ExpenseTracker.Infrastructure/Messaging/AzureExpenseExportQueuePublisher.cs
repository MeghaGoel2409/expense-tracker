using Azure.Storage.Queues;
using ExpenseTracker.Application.Features.ExpenseExports.DTOs;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;

namespace ExpenseTracker.Infrastructure.Messaging;

public sealed class AzureExpenseExportQueuePublisher
    : IExpenseExportQueuePublisher
{
    private readonly QueueClient _queueClient;

    public AzureExpenseExportQueuePublisher(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    public async Task PublishAsync(
        ExpenseExportQueueMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        await _queueClient.CreateIfNotExistsAsync(
            cancellationToken: cancellationToken);

        await _queueClient.SendMessageAsync(
            BinaryData.FromObjectAsJson(message),
            TimeSpan.Zero,
            TimeSpan.FromDays(1),
            cancellationToken);
    }
}