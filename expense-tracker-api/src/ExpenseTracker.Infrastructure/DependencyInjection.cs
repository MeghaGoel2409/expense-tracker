using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Features.ExpenseExports.Interfaces;
using ExpenseTracker.Application.Features.ExpenseExports.Services;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Infrastructure.Exports;
using ExpenseTracker.Infrastructure.Identity;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Repositories;
using ExpenseTracker.Infrastructure.ServiceCollectionExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddPersistence(configuration)
            .AddIdentityServices()
            .AddJwtAuthentication(configuration)
            .AddRepositories()
            .AddInfrastructureServices(configuration);
    }

    public static IServiceCollection AddExpenseExportInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IExportJobRepository, ExportJobRepository>();
        services.AddScoped<ICurrentUserService, FunctionCurrentUserService>();
        services.AddScoped<ICurrentUserSetter, FunctionCurrentUserService>();
        services.AddScoped<IExpenseExportFormatGenerator,CsvExpenseExportFormatGenerator>();
        services.AddScoped<IExpenseExportFileGenerator, ExpenseExportFileGenerator>();
        services.AddScoped<IExpenseExportProcessor, ExpenseExportProcessor>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddAzureStorage(configuration);

        return services;
    }
}