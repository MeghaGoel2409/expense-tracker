using ExpenseTracker.Application.Auth.Interfaces;
using ExpenseTracker.Application.Features.Categories.Commands;
using ExpenseTracker.Application.Features.Categories.Queries;
using ExpenseTracker.Application.Features.Categories.Validators;
using ExpenseTracker.Application.Features.Dashboard.Interfaces;
using ExpenseTracker.Application.Features.Dashboard.Queries;
using ExpenseTracker.Application.Features.Dashboard.Services;
using ExpenseTracker.Application.Features.Dashboard.Validators;
using ExpenseTracker.Application.Features.Expenses.Commands;
using ExpenseTracker.Application.Features.Expenses.Interfaces;
using ExpenseTracker.Application.Features.Expenses.Queries;
using ExpenseTracker.Application.Features.Expenses.Services;
using ExpenseTracker.Application.Features.Expenses.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateExpenseCommandHandler>();
        services.AddScoped<UpdateExpenseCommandHandler>();
        services.AddScoped<DeleteExpenseCommandHandler>();
        services.AddScoped<GetExpensesQueryHandler>();
        services.AddScoped<GetExpenseByIdQueryHandler>();
        services.AddScoped<IExpenseService, ExpenseService>();

        services.AddScoped<GetCategoriesQueryHandler>();
        services.AddScoped<CreateCategoryCommandHandler>();

        services.AddScoped<GetDashboardSummaryQueryHandler>();
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddScoped<IValidator<CreateExpenseCommand>, CreateExpenseCommandValidator>();
        services.AddScoped<IValidator<UpdateExpenseCommand>, UpdateExpenseCommandValidator>();
        services.AddScoped<IValidator<DeleteExpenseCommand>, DeleteExpenseCommandValidator>();
        services.AddScoped<IValidator<GetExpensesQuery>, GetExpensesQueryValidator>();

        services.AddScoped<IValidator<CreateCategoryCommand>, CreateCategoryCommandValidator>();

        services.AddScoped<IValidator<GetDashboardSummaryQuery>, GetDashboardSummaryQueryValidator>();

        return services;
    }
}