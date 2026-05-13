using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Categories.Commands;
using ExpenseTracker.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/categories")]

public class CategoriesController : BaseApiController
{
    private readonly GetCategoriesQueryHandler _getCategoriesQueryHandler;
    private readonly CreateCategoryCommandHandler _createCategoryHandler;

    public CategoriesController(GetCategoriesQueryHandler getCategoriesQueryHandler, CreateCategoryCommandHandler createCategoryHandler)
    {
        _getCategoriesQueryHandler = getCategoriesQueryHandler;
        _createCategoryHandler = createCategoryHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getCategoriesQueryHandler.Handle(new GetCategoriesQuery(), cancellationToken);

        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _createCategoryHandler.Handle(command, cancellationToken);

        return ToActionResult(result, StatusCodes.Status201Created);
    }
}