using ExpenseTracker.Application.Features.ExpenseExports.Models;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Features.ExpenseExports.Interfaces;

public interface IExpenseExportFormatGenerator
{
    ExportFormat Format { get; }

    GeneratedExportFile Generate(
        IReadOnlyList<Expense> expenses);
}