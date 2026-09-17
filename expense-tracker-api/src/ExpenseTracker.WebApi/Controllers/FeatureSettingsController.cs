using Asp.Versioning;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.FeatureSettings.DTOs;
using ExpenseTracker.Application.Features.FeatureSettings.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.WebApi.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/feature-settings")]
public sealed class FeatureSettingsController : BaseApiController
{
    private readonly IFeatureSettingsService _featureSettingsService;

    public FeatureSettingsController(IFeatureSettingsService featureSettingsService)
    {
        _featureSettingsService = featureSettingsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(FeatureSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _featureSettingsService.GetClientFeatureSettingsAsync(
            cancellationToken);

        return ToActionResult(result);
    }
}