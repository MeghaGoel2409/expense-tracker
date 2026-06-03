using ExpenseTracker.Application;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddSerilogLogging();
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebApi(builder.Configuration);

var app = builder.Build();

app.UseWebApiPipeline();
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var dbProvider = app.Configuration["DatabaseProvider"] ?? "SqlServer";

    try
    {
        if (dbProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            await dbContext.Database.EnsureCreatedAsync();
        }
        else
        {
            await dbContext.Database.MigrateAsync();
        }

        await ApplicationDbContextSeeder.SeedAsync(
            dbContext,
            scope.ServiceProvider,
            app.Environment.EnvironmentName);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed.");
        throw;
    }
}

app.Run();