namespace ExpenseTracker.Application.Common.Interfaces;

public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(
        string featureName,
        CancellationToken cancellationToken = default);
}