namespace ExpenseTracker.Application.Common.Models;

public class ValidationError : Error
{
    public string Field { get; }

    private ValidationError(string field, string code, string message)
        : base(code, message, ErrorType.Validation)
    {
        Field = field;
    }

    public static ValidationError Create(string field, string message, string? code = null)
        => new(field, code ?? "ValidationError", message);
}