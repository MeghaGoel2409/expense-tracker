using ExpenseTracker.Application.Features.ExpenseExports.Models;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Features.ExpenseExports.Interfaces;

public interface IExpenseExportFileGenerator
{
    GeneratedExportFile Generate(
        IReadOnlyList<Expense> expenses,
        ExportFormat format);
}