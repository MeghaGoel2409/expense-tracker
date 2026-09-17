using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using ExpenseTracker.Infrastructure.FeatureManagement;
using ExpenseTracker.Infrastructure.Identity;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Infrastructure.ServiceCollectionExtensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFeatureFlagService, FeatureFlagService>();
        services.AddAzureStorage(configuration);
        services.AddHttpContextAccessor();

        return services;
    }

}