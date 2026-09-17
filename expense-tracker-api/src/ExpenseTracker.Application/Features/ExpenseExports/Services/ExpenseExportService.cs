using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.ExpenseExports.DTOs;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using ExpenseTracker.Application.Features.ExpenseExports.Models;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentValidation;
using System.Diagnostics;
using System.Text.Json;

namespace ExpenseTracker.Application.Features.ExpenseExports.Services;

public sealed class ExpenseExportService : IExpenseExportService
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IExportJobRepository _exportJobRepository;
    private readonly IExpenseExportQueuePublisher _queuePublisher;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateExpenseExportRequest> _validator;

    private readonly IExpenseExportStorage _storage;

    public ExpenseExportService(
        IExportJobRepository exportJobRepository,
        IExpenseExportQueuePublisher queuePublisher,
        IExpenseExportStorage storage,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IValidator<CreateExpenseExportRequest> validator)
    {
        _exportJobRepository = exportJobRepository;
        _queuePublisher = queuePublisher;
        _storage = storage;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<ExpenseExportStatusResponse>> GetStatusAsync(
    int exportJobId,
    CancellationToken cancellationToken = default)
    {
        if (exportJobId <= 0)
        {
            return Result<ExpenseExportStatusResponse>.Failure(
                Error.Validation(
                    nameof(exportJobId),
                    "Export job ID must be greater than zero."));
        }

        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "A signed-in user is required to view an export.");
        }

        var exportJob = await _exportJobRepository.GetByIdForUserAsync(
            exportJobId,
            userId,
            cancellationToken);

        if (exportJob is null)
        {
            return Result<ExpenseExportStatusResponse>.Failure(
                Error.NotFound(
                    "ExpenseExport.NotFound",
                    "The requested expense export was not found."));
        }

        return new ExpenseExportStatusResponse(
            exportJob.Id,
            exportJob.Status,
            exportJob.ErrorMessage,
            exportJob.CreatedOnUtc,
            exportJob.StartedOnUtc,
            exportJob.CompletedOnUtc);
    }

    public async Task<Result<CreateExpenseExportResponse>> CreateAsync(
        CreateExpenseExportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationResult = await _validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => Error.Validation(
                    error.PropertyName,
                    error.ErrorMessage))
                .ToList();

            return Result<CreateExpenseExportResponse>.Failure(errors);
        }

        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "A signed-in user is required to create an export.");
        }

        var filtersJson = JsonSerializer.Serialize(
            request.Filter,
            SerializerOptions);

        var existingJob = await _exportJobRepository.GetActiveMatchingJobAsync(userId, request.Format,
            filtersJson,
            cancellationToken);

        if (existingJob is not null)
        {
            return new CreateExpenseExportResponse(existingJob.Id);
        }

        var exportJob = new ExportJob(
            userId,
            request.Format,
            filtersJson);

        await _exportJobRepository.AddAsync(
            exportJob,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var correlationId =
            Activity.Current?.TraceId.ToString()
            ?? Guid.NewGuid().ToString("N");

        var queueMessage = new ExpenseExportQueueMessage(
            exportJob.Id,
            userId,
            correlationId);

        try
        {
            await _queuePublisher.PublishAsync(
                queueMessage,
                cancellationToken);
        }
        catch (Exception ex)
        {
            exportJob.MarkAsFailed(
                $"The export request could not be queued: {ex.Message}");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw;
        }

        return new CreateExpenseExportResponse(exportJob.Id);
    }

    public async Task<Result<GeneratedExportFile>> DownloadAsync(
    int exportJobId,
    CancellationToken cancellationToken = default)
    {
        if (exportJobId <= 0)
        {
            return Result<GeneratedExportFile>.Failure(
                Error.Validation(
                    nameof(exportJobId),
                    "Export job ID must be greater than zero."));
        }

        var userId = _currentUserService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "A signed-in user is required to download an export.");
        }

        var exportJob = await _exportJobRepository.GetByIdForUserAsync(
            exportJobId,
            userId,
            cancellationToken);

        if (exportJob is null)
        {
            return Result<GeneratedExportFile>.Failure(
                Error.NotFound(
                    "ExpenseExport.NotFound",
                    "The requested expense export was not found."));
        }

        if (exportJob.Status != ExportJobStatus.Completed ||
            string.IsNullOrWhiteSpace(exportJob.BlobName))
        {
            return Result<GeneratedExportFile>.Failure(
                Error.Conflict(
                    "ExpenseExport.NotReady",
                    "The expense export is not ready for download."));
        }

        var file = await _storage.DownloadAsync(
            exportJob.BlobName,
            cancellationToken);

        return file;
    }
}
