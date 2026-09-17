using System.Globalization;
using System.Text;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using ExpenseTracker.Application.Features.ExpenseExports.Models;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Infrastructure.Exports;

public sealed class CsvExpenseExportFormatGenerator: IExpenseExportFormatGenerator
{
    public ExportFormat Format => ExportFormat.Csv;

    public GeneratedExportFile Generate(
        IReadOnlyList<Expense> expenses)
    {
        ArgumentNullException.ThrowIfNull(expenses);

        var csv = new StringBuilder();

        csv.AppendLine(
            "Date,Category,Merchant,Amount,Currency," +
            "Payment Method,Expense Type,Recurring,Notes");

        foreach (var expense in expenses)
        {
            csv.Append(Escape(
                expense.ExpenseDate.ToString(
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture)));

            csv.Append(',');
            csv.Append(Escape(expense.Category?.Name));

            csv.Append(',');
            csv.Append(Escape(expense.Merchant));

            csv.Append(',');
            csv.Append(expense.Amount.ToString(
                "0.00",
                CultureInfo.InvariantCulture));

            csv.Append(',');
            csv.Append(Escape(expense.Currency));

            csv.Append(',');
            csv.Append(Escape(expense.PaymentMethod));

            csv.Append(',');
            csv.Append(Escape(expense.ExpenseType.ToString()));

            csv.Append(',');
            csv.Append(expense.IsRecurring ? "Yes" : "No");

            csv.Append(',');
            csv.Append(Escape(expense.Notes));

            csv.AppendLine();
        }

        var utf8 = new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: true);

        var content = utf8.GetPreamble()
            .Concat(utf8.GetBytes(csv.ToString()))
            .ToArray();

        return new GeneratedExportFile(
            content,
            $"expenses-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv",
            "text/csv");
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var firstNonWhitespaceCharacter =
            value.FirstOrDefault(character => !char.IsWhiteSpace(character));

        var safeValue =
            firstNonWhitespaceCharacter is '=' or '+' or '-' or '@'
                ? $"'{value}"
                : value;

        return $"\"{safeValue.Replace("\"", "\"\"")}\"";
    }
}