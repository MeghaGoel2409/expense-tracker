using System.Text.Json;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Features.ExpenseExports.DTOs;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Application.Features.Expenses.Models;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Features.ExpenseExports.Services;

public sealed class ExpenseExportProcessor : IExpenseExportProcessor
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    private readonly IExportJobRepository _exportJobRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IExpenseExportFileGenerator _fileGenerator;
    private readonly IExpenseExportStorage _storage;
    private readonly IUnitOfWork _unitOfWork;

    public ExpenseExportProcessor(
        IExportJobRepository exportJobRepository,
        IExpenseRepository expenseRepository,
        IExpenseExportFileGenerator fileGenerator,
        IExpenseExportStorage storage,
        IUnitOfWork unitOfWork)
    {
        _exportJobRepository = exportJobRepository;
        _expenseRepository = expenseRepository;
        _fileGenerator = fileGenerator;
        _storage = storage;
        _unitOfWork = unitOfWork;
    }

    public async Task ProcessAsync(
        ExpenseExportQueueMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var exportJob =
            await _exportJobRepository.GetByIdForUserAsync(
                message.ExportJobId,
                message.UserId,
                cancellationToken);

        if (exportJob is null)
        {
            throw new InvalidOperationException(
                $"Export job {message.ExportJobId} was not found.");
        }

        // Message was already successfully processed.
        if (exportJob.Status == ExportJobStatus.Completed)
        {
            return;
        }

        try
        {
            exportJob.MarkAsProcessing();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var filter =
                JsonSerializer.Deserialize<ExpenseExportFilterDto>(
                    exportJob.FiltersJson,
                    SerializerOptions)
                ?? throw new InvalidOperationException(
                    $"Filters for export job {exportJob.Id} " +
                    "could not be deserialized.");

            var queryFilter = new ExpenseQueryFilter
            {
                UserId = message.UserId,
                CategoryId = filter.CategoryId,

                FromDate = filter.FromDate?
                    .ToDateTime(TimeOnly.MinValue),

                ToDate = filter.ToDate?
                    .ToDateTime(TimeOnly.MaxValue),

                SortBy = "date",
                SortDescending = true
            };

            var expenses =
                await _expenseRepository.GetByFilterAsync(
                    queryFilter,
                    cancellationToken);

            var file = _fileGenerator.Generate(
                expenses,
                exportJob.Format);

            var blobName = await _storage.UploadAsync(
                message.UserId,
                exportJob.Id,
                file,
                cancellationToken);

            exportJob.MarkAsCompleted(blobName);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (Exception)
        {
            if (exportJob.Status != ExportJobStatus.Completed)
            {
                exportJob.MarkAsFailed(
                    "Expense export processing failed.");

                await _unitOfWork.SaveChangesAsync(
                    CancellationToken.None);
            }

            throw;
        }
    }
}