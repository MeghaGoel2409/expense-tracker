using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using ExpenseTracker.Application.Features.ExpenseExports.Models;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Infrastructure.Exports;

public sealed class ExpenseExportFileGenerator
    : IExpenseExportFileGenerator
{
    private readonly IReadOnlyDictionary<ExportFormat, IExpenseExportFormatGenerator> _generators;

    public ExpenseExportFileGenerator(
        IEnumerable<IExpenseExportFormatGenerator> generators)
    {
        ArgumentNullException.ThrowIfNull(generators);

        _generators = generators.ToDictionary(
            generator => generator.Format);
    }

    public GeneratedExportFile Generate(
        IReadOnlyList<Expense> expenses,
        ExportFormat format)
    {
        ArgumentNullException.ThrowIfNull(expenses);

        if (!_generators.TryGetValue(format, out var generator))
        {
            throw new NotSupportedException(
                $"Export format '{format}' is not supported.");
        }

        return generator.Generate(expenses);
    }
}