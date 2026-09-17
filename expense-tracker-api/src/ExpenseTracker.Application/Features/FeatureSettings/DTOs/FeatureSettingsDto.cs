namespace ExpenseTracker.Application.Features.FeatureSettings.DTOs;

public sealed class FeatureSettingsDto
{
    public Dictionary<string, bool> Features { get; init; } = new();
}