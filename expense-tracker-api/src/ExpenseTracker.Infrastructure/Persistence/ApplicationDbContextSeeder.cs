using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Persistence;

public static class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IServiceProvider serviceProvider,
        string environmentName)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        await SeedCategoriesAsync(context, logger);

        if (environmentName is "Development" or "Test" or "Demo")
        {
            await DemoDataSeeder.SeedAsync(context, serviceProvider);
        }
    }

    private static async Task SeedCategoriesAsync(
        ApplicationDbContext context,
        ILogger logger)
    {
        if (await context.Categories.AnyAsync(x => x.IsSystemDefined))
        {
            logger.LogDebug("System categories seed skipped because categories already exist.");
            return;
        }

        var categories = new[]
        {
            Category.CreateSystem("Rent", "Monthly rent payment", "home"),
            Category.CreateSystem("Groceries", "Everyday grocery shopping", "shopping-cart"),
            Category.CreateSystem("Dining", "Restaurants, cafes, and takeout", "utensils"),
            Category.CreateSystem("Transport", "Fuel, rides, and public transport", "car"),
            Category.CreateSystem("Entertainment", "Movies, streaming, and fun activities", "film"),
            Category.CreateSystem("Utilities", "Electricity, water, internet, phone bills", "bolt"),
            Category.CreateSystem("Health", "Medical, pharmacy, and wellness", "heartbeat"),
            Category.CreateSystem("Shopping", "Clothing, electronics, and general shopping", "bag-shopping"),
            Category.CreateSystem("Education", "Courses, books, and learning", "book"),
            Category.CreateSystem("Travel", "Flights, hotels, and trips", "plane"),
            Category.CreateSystem("Personal Care", "Salon, grooming, and self-care", "spa"),
            Category.CreateSystem("Insurance", "Health, car, and other insurance payments", "shield"),
            Category.CreateSystem("Subscriptions", "Recurring subscriptions and memberships", "repeat"),
            Category.CreateSystem("Miscellaneous", "Other uncategorized expenses", "ellipsis")
        };  

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {CategoryCount} system categories.", categories.Length);
    }
}