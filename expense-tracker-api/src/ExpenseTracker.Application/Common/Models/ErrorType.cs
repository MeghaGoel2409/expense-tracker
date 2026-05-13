namespace ExpenseTracker.Application.Common.Models;

public enum ErrorType
{
    Validation = 1,
    NotFound = 2,
    Forbidden = 3,
    Unauthorized = 4,
    Conflict = 5,
    Failure = 6,
    Database =7,
}