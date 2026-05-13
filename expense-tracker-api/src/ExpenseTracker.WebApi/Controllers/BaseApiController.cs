using ExpenseTracker.Application.Common.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;
[ApiController]
[ProducesResponseType(typeof(Result), StatusCodes.Status500InternalServerError)]

public abstract class BaseApiController : ControllerBase
{
    protected IActionResult ToActionResult(Result result, int successStatusCode = StatusCodes.Status200OK)
    {
        if (!result.IsSuccess)
        {
            return MapFailure(result);
        }

        return successStatusCode == StatusCodes.Status204NoContent
            ? NoContent()
            : StatusCode(successStatusCode, result);
    }

    protected IActionResult ToActionResult<T>(Result<T> result, int successStatusCode = StatusCodes.Status200OK)
    {
        if (!result.IsSuccess)
        {
            return MapFailure(result);
        }

        return successStatusCode == StatusCodes.Status204NoContent
            ? NoContent()
            : StatusCode(successStatusCode, result);
    }

    protected IActionResult ValidationFailure(string code, string message)
    {
        return ToActionResult(Result.Failure(Error.Validation(code, message)));
    }

    protected IActionResult ValidationFailure(IEnumerable<ValidationFailure> failures)
    {
        var errors = failures
            .Select(x => Error.Validation(x.PropertyName, x.ErrorMessage))
            .ToList();

        return ToActionResult(Result.Failure(errors));
    }

    protected IActionResult ValidationFailure(IEnumerable<Error> errors)
    {
        return ToActionResult(Result.Failure(errors));
    }

    private IActionResult MapFailure(Result result)
    {
        var firstError = result.Errors.FirstOrDefault();

        if (firstError is null)
        {
            return BadRequest(result);
        }

        return firstError.Type switch
        {
            ErrorType.Validation => BadRequest(result),
            ErrorType.Unauthorized => Unauthorized(result),
            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result),
            ErrorType.NotFound => NotFound(result),
            ErrorType.Conflict => Conflict(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }
}