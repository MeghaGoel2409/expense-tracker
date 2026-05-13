using System.Text.Json.Serialization;

namespace ExpenseTracker.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; protected set; }

    public List<Error> Errors { get; protected set; } = new();

    public bool HasErrors => Errors.Count > 0;

    public string? TraceId { get; set; }

    public static Result Success()
    {
        return new Result
        {
            IsSuccess = true
        };
    }

    public static Result Failure(params Error[] errors)
    {
        return new Result
        {
            IsSuccess = false,
            Errors = errors.ToList()
        };
    }

    public static Result Failure(IEnumerable<Error> errors)
    {
        return new Result
        {
            IsSuccess = false,
            Errors = errors.ToList()
        };
    }

    public static implicit operator Result(Error error)
    {
        return Failure(error);
    }

    public static implicit operator Result(List<Error> errors)
    {
        return Failure(errors);
    }

    public static implicit operator Result(Error[] errors)
    {
        return Failure(errors);
    }

}

public class Result<T> : Result
{
    public T? Data { get; private set; } //internal storage

    [JsonIgnore]
    public T Value => IsSuccess
        ? Data!
        : throw new InvalidOperationException("Cannot access Value when result is failure.");


    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public new static Result<T> Failure(params Error[] errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = errors.ToList()
        };
    }

    public new static Result<T> Failure(IEnumerable<Error> errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = errors.ToList()
        };
    }

    public static implicit operator Result<T>(T data)
    {
        return Success(data);
    }

    public static implicit operator Result<T>(Error error)
    {
        return Failure(error);
    }

    public static implicit operator Result<T>(List<Error> errors)
    {
        return Failure(errors);
    }

    public static implicit operator Result<T>(Error[] errors)
    {
        return Failure(errors);
    }
}