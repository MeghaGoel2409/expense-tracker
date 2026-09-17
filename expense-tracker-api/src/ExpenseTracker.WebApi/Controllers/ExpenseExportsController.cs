using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.ExpenseExports.DTOs;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace ExpenseTracker.WebApi.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/expense-exports")]
public sealed class ExpenseExportsController : BaseApiController
{
    private readonly IExpenseExportService _expenseExportService;
    private readonly IFeatureManager _featureManager;

    public ExpenseExportsController(
        IExpenseExportService expenseExportService,
        IFeatureManager featureManager)
    {
        _expenseExportService = expenseExportService;
        _featureManager = featureManager;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(Result<CreateExpenseExportResponse>),
        StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateExpenseExportRequest request,
        CancellationToken cancellationToken)
    {
        if (!await _featureManager.IsEnabledAsync(
                FeatureFlags.ExpenseExport))
        {
            return ToActionResult(
                Result.Failure(
                    Error.Forbidden(
                        "Feature.Export.Disabled",
                        "Expense export is not enabled.")));
        }

        var result = await _expenseExportService.CreateAsync(
            request,
            cancellationToken);

        return ToActionResult(
            result,
            successStatusCode: StatusCodes.Status202Accepted);
    }

    [HttpGet("{exportJobId:int}")]
    [ProducesResponseType(
    typeof(Result<ExpenseExportStatusResponse>),
    StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(int exportJobId, CancellationToken cancellationToken)
    {
        var result = await _expenseExportService.GetStatusAsync(
            exportJobId,
            cancellationToken);

        return ToActionResult(result);
    }

    [HttpGet("{exportJobId:int}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Download(int exportJobId, CancellationToken cancellationToken)
    {
        var result = await _expenseExportService.DownloadAsync(exportJobId, cancellationToken);

        if (!result.IsSuccess)
        {
            return ToActionResult(result);
        }

        var file = result.Value;

        return File(file.Content, file.ContentType, file.FileName);
    }
}