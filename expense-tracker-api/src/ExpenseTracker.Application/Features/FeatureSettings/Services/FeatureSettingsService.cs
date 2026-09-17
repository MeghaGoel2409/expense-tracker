using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.FeatureSettings.DTOs;
using ExpenseTracker.Application.Features.FeatureSettings.Interfaces;

namespace ExpenseTracker.Application.Features.FeatureSettings.Services;

public sealed class FeatureSettingsService : IFeatureSettingsService
{
    private readonly IFeatureFlagService _featureFlagService;

    public FeatureSettingsService(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService;
    }

    public async Task<Result<FeatureSettingsDto>> GetClientFeatureSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        var features = new Dictionary<string, bool>();

        foreach (var feature in FeatureFlags.ClientFeatures)
        {
            features[feature.Key] = await _featureFlagService.IsEnabledAsync(
                feature.Value,
                cancellationToken);
        }

        return Result<FeatureSettingsDto>.Success(new FeatureSettingsDto
        {
            Features = features
        });
    }
}