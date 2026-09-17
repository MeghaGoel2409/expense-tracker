using ExpenseTracker.Application.Features.ExpenseExports.DTOs;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Functions.Functions;

public sealed class ProcessExpenseExportFunction
{
    private readonly IExpenseExportProcessor _processor;
    private readonly ILogger<ProcessExpenseExportFunction> _logger;

    public ProcessExpenseExportFunction(
        IExpenseExportProcessor processor,
        ILogger<ProcessExpenseExportFunction> logger)
    {
        _processor = processor;
        _logger = logger;
    }

    [Function(nameof(ProcessExpenseExportFunction))]
    public async Task RunAsync(
        [QueueTrigger(
            "%ExpenseExportsQueueName%",
            Connection = "AzureWebJobsStorage")]
        ExpenseExportQueueMessage message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing expense export job {ExportJobId}. " +
            "UserId: {UserId}, CorrelationId: {CorrelationId}",
            message.ExportJobId,
            message.UserId,
            message.CorrelationId);

        try
        {
            await _processor.ProcessAsync(
                message,
                cancellationToken);

            _logger.LogInformation(
                "Expense export job {ExportJobId} completed.",
                message.ExportJobId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Expense export job {ExportJobId} failed. " +
                "CorrelationId: {CorrelationId}",
                message.ExportJobId,
                message.CorrelationId);

            throw;
        }
    }
}