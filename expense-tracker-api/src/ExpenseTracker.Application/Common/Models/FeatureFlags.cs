namespace ExpenseTracker.Application.Common.Models;

public static class FeatureFlags
{
    public const string ExpenseExport = "ExpenseExport";

    public static readonly IReadOnlyDictionary<string, string> ClientFeatures =
        new Dictionary<string, string>
        {
            ["expenseExport"] = ExpenseExport
        };
}