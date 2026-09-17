using ExpenseTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure.ServiceCollectionExtensions;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseProvider = configuration["DatabaseProvider"] ?? "SqlServer";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                var sqliteConnectionString =
                    configuration.GetConnectionString("SqliteConnection")
                    ?? "Data Source=C:\\home\\site\\ExpenseTracker.db";

                options.UseSqlite(sqliteConnectionString);
            }
            else
            {
                var sqlServerConnectionString =
                    configuration.GetConnectionString("DefaultConnection");

                options.UseSqlServer(sqlServerConnectionString);
            }
        });

        return services;
    }
}