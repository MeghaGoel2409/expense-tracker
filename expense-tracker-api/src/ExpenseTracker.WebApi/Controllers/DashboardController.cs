using Asp.Versioning;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.Dashboard.DTOs;
using ExpenseTracker.Application.Features.Dashboard.Interfaces;
using ExpenseTracker.Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/dashboard")]
public sealed class DashboardController : BaseApiController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Gets dashboard summary for the logged-in user.
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(Result<DashboardSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] GetDashboardSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetSummaryAsync(query, cancellationToken);
        return ToActionResult(result);
    }
}