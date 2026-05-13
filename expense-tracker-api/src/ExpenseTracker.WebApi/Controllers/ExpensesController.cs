using ExpenseTracker.Application.Auth.DTOs;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Expenses.Commands;
using ExpenseTracker.Application.Features.Expenses.DTOs;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Application.Features.Expenses.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/expenses")]

public class ExpensesController : BaseApiController
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(
       IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<ExpenseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateExpenseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _expenseService.CreateAsync(command, cancellationToken);

        return ToActionResult(result, successStatusCode: StatusCodes.Status201Created);
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<ExpenseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetExpensesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _expenseService.GetExpensesAsync(query, cancellationToken);

        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<ExpenseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _expenseService.GetExpenseByIdAsync(new GetExpenseByIdQuery(id), cancellationToken);

        return ToActionResult(result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Result<ExpenseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateExpenseCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return ValidationFailure("Expense.IdMismatch", "Route id does not match request id.");
        }       

        var result = await _expenseService.UpdateAsync(command, cancellationToken);

        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteExpenseCommand { Id = id };

        var result = await _expenseService.DeleteAsync(command, cancellationToken);

        return ToActionResult(result);
    }
}