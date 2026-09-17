using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Features.FeatureSettings.DTOs;

namespace ExpenseTracker.Application.Features.FeatureSettings.Interfaces;

public interface IFeatureSettingsService
{
    Task<Result<FeatureSettingsDto>> GetClientFeatureSettingsAsync(
        CancellationToken cancellationToken = default);
}