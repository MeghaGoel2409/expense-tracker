namespace ExpenseTracker.Application.Features.ExpenseExports.Models;

public sealed record GeneratedExportFile(
    byte[] Content,
    string FileName,
    string ContentType);