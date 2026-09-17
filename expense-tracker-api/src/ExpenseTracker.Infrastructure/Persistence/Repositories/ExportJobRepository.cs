using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories;

public sealed class ExportJobRepository : IExportJobRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ExportJobRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        ExportJob exportJob,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exportJob);

        await _dbContext.ExportJobs.AddAsync(
            exportJob,
            cancellationToken);
    }

    public Task<ExportJob?> GetByIdAsync(
        int exportJobId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.ExportJobs
            .FirstOrDefaultAsync(
                x => x.Id == exportJobId &&
                     !x.IsDeleted,
                cancellationToken);
    }

    public Task<ExportJob?> GetByIdForUserAsync(
        int exportJobId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        return _dbContext.ExportJobs
            .FirstOrDefaultAsync(
                x => x.Id == exportJobId &&
                     x.UserId == userId &&
                     !x.IsDeleted,
                cancellationToken);
    }

    public Task<ExportJob?> GetActiveMatchingJobAsync(
    string userId,
    ExportFormat format,
    string filtersJson,
    CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(filtersJson);

        return _dbContext.ExportJobs
            .AsNoTracking()
            .Where(exportJob =>
                exportJob.UserId == userId &&
                exportJob.Format == format &&
                exportJob.FiltersJson == filtersJson &&
                !exportJob.IsDeleted &&
                (exportJob.Status == ExportJobStatus.Pending ||
                 exportJob.Status == ExportJobStatus.Processing))
            .OrderByDescending(exportJob => exportJob.CreatedOnUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }
}