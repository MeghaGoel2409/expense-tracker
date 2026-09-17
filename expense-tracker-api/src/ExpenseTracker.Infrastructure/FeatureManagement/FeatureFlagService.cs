using ExpenseTracker.Application.Common.Interfaces;
using Microsoft.FeatureManagement;

namespace ExpenseTracker.Infrastructure.FeatureManagement;

public sealed class FeatureFlagService : IFeatureFlagService
{
    private readonly IFeatureManager _featureManager;

    public FeatureFlagService(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    public Task<bool> IsEnabledAsync(
        string featureName,
        CancellationToken cancellationToken = default)
    {
        return _featureManager.IsEnabledAsync(featureName, cancellationToken);
    }
}