using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Features.ExpenseExports.Interfaces;

public interface IExportJobRepository
{
    Task AddAsync(
        ExportJob exportJob,
        CancellationToken cancellationToken = default);

    Task<ExportJob?> GetByIdAsync(
        int exportJobId,
        CancellationToken cancellationToken = default);

    Task<ExportJob?> GetByIdForUserAsync(
        int exportJobId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<ExportJob?> GetActiveMatchingJobAsync(
    string userId,
    ExportFormat format,
    string filtersJson,
    CancellationToken cancellationToken = default);
}