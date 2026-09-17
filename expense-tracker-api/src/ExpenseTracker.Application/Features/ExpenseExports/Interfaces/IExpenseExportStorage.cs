using ExpenseTracker.Application.Features.ExpenseExports.Models;

namespace ExpenseTracker.Application.Features.ExpenseExports.Interfaces;

public interface IExpenseExportStorage
{
    Task<string> UploadAsync(string userId, int exportJobId, GeneratedExportFile file,
                            CancellationToken cancellationToken = default);

    Task<GeneratedExportFile> DownloadAsync(string blobName, CancellationToken cancellationToken = default);
}