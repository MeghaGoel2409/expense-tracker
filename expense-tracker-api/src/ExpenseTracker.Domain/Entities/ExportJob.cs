using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

public sealed class ExportJob : BaseAuditableEntity
{
    private ExportJob()
    {
    }

    public ExportJob(
        string userId,
        ExportFormat format,
        string filtersJson)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(filtersJson))
        {
            throw new ArgumentException(
                "Export filters are required.",
                nameof(filtersJson));
        }

        UserId = userId;
        Format = format;
        FiltersJson = filtersJson;
        Status = ExportJobStatus.Pending;
    }

    public string UserId { get; private set; } = default!;

    public ExportFormat Format { get; private set; }

    public ExportJobStatus Status { get; private set; }

    public string FiltersJson { get; private set; } = default!;

    public string? BlobName { get; private set; }

    public string? ErrorMessage { get; private set; }

    public DateTime? StartedOnUtc { get; private set; }

    public DateTime? CompletedOnUtc { get; private set; }

    public void MarkAsProcessing()
    {
        if (Status is not (
            ExportJobStatus.Pending or
            ExportJobStatus.Processing or
            ExportJobStatus.Failed))
        {
            throw new InvalidOperationException(
                $"An export job in status '{Status}' cannot be started.");
        }

        if (Status != ExportJobStatus.Processing)
        {
            StartedOnUtc = DateTime.UtcNow;
        }

        Status = ExportJobStatus.Processing;
        CompletedOnUtc = null;
        ErrorMessage = null;
    }

    public void MarkAsCompleted(string blobName)
    {
        if (Status != ExportJobStatus.Processing)
        {
            throw new InvalidOperationException(
                $"An export job in status '{Status}' cannot be completed.");
        }

        if (string.IsNullOrWhiteSpace(blobName))
        {
            throw new ArgumentException(
                "Blob name is required.",
                nameof(blobName));
        }

        Status = ExportJobStatus.Completed;
        BlobName = blobName.Trim();
        ErrorMessage = null;
        CompletedOnUtc = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        if (Status is ExportJobStatus.Completed or ExportJobStatus.Failed)
        {
            throw new InvalidOperationException(
                $"An export job in status '{Status}' cannot be marked as failed.");
        }

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException(
                "Error message is required.",
                nameof(errorMessage));
        }

        Status = ExportJobStatus.Failed;
        ErrorMessage = errorMessage.Trim();
        CompletedOnUtc = DateTime.UtcNow;
    }
}