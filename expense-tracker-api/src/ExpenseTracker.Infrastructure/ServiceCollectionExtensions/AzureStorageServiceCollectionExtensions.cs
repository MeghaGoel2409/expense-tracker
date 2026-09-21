using Azure.Core;
using Azure.Identity;
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
        var accountName = configuration["AzureWebJobsStorage:accountName"];
        var queueName = configuration["ExpenseExportsQueueName"];

        if (string.IsNullOrWhiteSpace(connectionString) &&
            string.IsNullOrWhiteSpace(accountName))
        {
            throw new InvalidOperationException(
                "Configure either 'AzureWebJobsStorage' for local development " +
                "or 'AzureWebJobsStorage:accountName' for Azure.");
        }

        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException(
                "ExpenseExportsQueueName is not configured.");
        }

        services.AddSingleton<TokenCredential>(_ =>
            new DefaultAzureCredential());

        services.AddSingleton(sp =>
        {
            var clientOptions = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return new QueueClient(
                    connectionString,
                    queueName,
                    clientOptions);
            }

            var queueUri = new Uri(
                $"https://{accountName}.queue.core.windows.net/{queueName}");

            return new QueueClient(
                queueUri,
                sp.GetRequiredService<TokenCredential>(),
                clientOptions);
        });

        services.AddSingleton(sp =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                return new BlobServiceClient(connectionString);
            }

            var blobServiceUri = new Uri(
                $"https://{accountName}.blob.core.windows.net");

            return new BlobServiceClient(
                blobServiceUri,
                sp.GetRequiredService<TokenCredential>());
        });

        services.AddScoped<
            IExpenseExportQueuePublisher,
            AzureExpenseExportQueuePublisher>();

        services.AddScoped<
            IExpenseExportStorage,
            AzureExpenseExportStorage>();

        return services;
    }
}