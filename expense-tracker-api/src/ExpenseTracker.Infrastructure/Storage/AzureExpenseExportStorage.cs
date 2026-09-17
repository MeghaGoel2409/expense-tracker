using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using ExpenseTracker.Application.Features.ExpenseExports.Models;

namespace ExpenseTracker.Infrastructure.Storage;

public sealed class AzureExpenseExportStorage: IExpenseExportStorage
{
    private const string ContainerName = "expense-exports";

    private readonly BlobServiceClient _blobServiceClient;

    public AzureExpenseExportStorage(
        BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadAsync(
        string userId,
        int exportJobId,
        GeneratedExportFile file,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentNullException.ThrowIfNull(file);

        var containerClient =
            _blobServiceClient.GetBlobContainerClient(ContainerName);

        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.None,
            cancellationToken: cancellationToken);

        var blobName =
            $"{userId}/{exportJobId}/{file.FileName}";

        var blobClient =
            containerClient.GetBlobClient(blobName);

        using var stream = new MemoryStream(file.Content);

        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                }
            },
            cancellationToken);

        return blobName;
    }

    public async Task<GeneratedExportFile> DownloadAsync(string blobName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(blobName);

        var downloadResult = await blobClient.DownloadContentAsync(cancellationToken);

        var contentType = downloadResult.Value.Details.ContentType ?? "application/octet-stream";

        var fileName = Path.GetFileName(blobName);

        return new GeneratedExportFile(downloadResult.Value.Content.ToArray(), fileName, contentType);
    }
}