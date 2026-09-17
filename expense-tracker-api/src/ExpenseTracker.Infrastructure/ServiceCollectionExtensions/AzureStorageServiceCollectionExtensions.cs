using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using ExpenseTracker.Infrastructure.Messaging;
using ExpenseTracker.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure.ServiceCollectionExtensions;

public static class AzureStorageServiceCollectionExtensions
{
    public static IServiceCollection AddAzureStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["AzureWebJobsStorage"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "AzureWebJobsStorage is not configured.");
        }

        var queueName = configuration["ExpenseExportsQueueName"];

        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException(
                "ExpenseExportsQueueName is not configured.");
        }

        services.AddSingleton(_ =>
        {
            var clientOptions = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            return new QueueClient(
                connectionString,
                queueName,
                clientOptions);
        });

        services.AddScoped<
            IExpenseExportQueuePublisher,
            AzureExpenseExportQueuePublisher>();

        services.AddSingleton(
            _ => new BlobServiceClient(connectionString));

        services.AddScoped<
            IExpenseExportStorage,
            AzureExpenseExportStorage>();

        return services;
    }
}