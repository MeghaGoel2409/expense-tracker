using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.ExpenseExports.DTOs;
using ExpenseTracker.Application.Features.ExpenseExports.Models;

namespace ExpenseTracker.Application.Features.ExpenseExports.Interfaces;

public interface IExpenseExportService
{
    Task<Result<CreateExpenseExportResponse>> CreateAsync(
        CreateExpenseExportRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<ExpenseExportStatusResponse>> GetStatusAsync(
        int exportJobId,
        CancellationToken cancellationToken = default);

    Task<Result<GeneratedExportFile>> DownloadAsync(
    int exportJobId,
    CancellationToken cancellationToken = default);
}