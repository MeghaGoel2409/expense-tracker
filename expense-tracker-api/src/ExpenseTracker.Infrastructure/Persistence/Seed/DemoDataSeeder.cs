using Bogus;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Infrastructure.Persistence.Seed;

public static class DemoDataSeeder
{
    private const string DemoEmail = "demo@expensetracker.com";
    private const string DemoPassword = "Demo@123";

    public static async Task SeedAsync(
        ApplicationDbContext context,
        IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var demoUser = await userManager.FindByEmailAsync(DemoEmail);

        if (demoUser is null)
        {
            demoUser = new ApplicationUser
            {
                UserName = DemoEmail,
                Email = DemoEmail,
                FirstName = "Demo",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(demoUser, DemoPassword);

            if (!result.Succeeded)
            {
                logger.LogError("Failed to create demo user. ErrorCodes: {ErrorCodes}", result.Errors.Select(x => x.Code));
                throw new InvalidOperationException("Failed to create demo user.");
            }

            logger.LogInformation("Demo user created successfully. UserId: {UserId}", demoUser.Id);
        }

        if (await context.Expenses.AnyAsync(x => x.UserId == demoUser.Id))
        {
            logger.LogDebug("Demo data seed skipped because demo expenses already exist. UserId: {UserId}", demoUser.Id);
            return;
        }

        var categories = await context.Categories
            .Where(x => x.IsSystemDefined)
            .ToListAsync();

        await SeedExpensesAsync(context, demoUser.Id, categories);
        await SeedIncomeAsync(context, demoUser.Id);
        await SeedBudgetsAsync(context, demoUser.Id, categories);

        await context.SaveChangesAsync();

        logger.LogInformation("Demo data seeded successfully. UserId: {UserId}", demoUser.Id);
    }

    private static async Task SeedExpensesAsync(
        ApplicationDbContext context,
        string userId,
        List<Category> categories)
    {
        var faker = new Faker();
        var expenses = new List<Expense>();

        var paymentMethods = new[]
        {
            "Credit Card",
            "Debit Card",
            "Bank Transfer",
            "Cash"
        };

        var today = DateTime.Today;
        var sixMonthsAgo = new DateTime(today.AddMonths(-5).Year, today.AddMonths(-5).Month, 1);

        // Fixed monthly household expenses. These make the demo data feel realistic
        // and also ensure there is only one rent expense per month.
        for (var i = 5; i >= 0; i--)
        {
            var month = today.AddMonths(-i);
            var year = month.Year;
            var monthNumber = month.Month;

            AddExpenseIfCategoryExists(
                expenses,
                categories,
                categoryName: "Rent",
                amount: 1850,
                expenseDate: SafeDate(year, monthNumber, 1),
                userId: userId,
                merchant: "Avalon Apartments",
                paymentMethod: "Bank Transfer",
                expenseType: ExpenseType.Essential,
                isRecurring: true,
                notes: "Monthly rent");

            AddExpenseIfCategoryExists(
                expenses,
                categories,
                categoryName: "Utilities",
                amount: faker.Random.Decimal(105, 185),
                expenseDate: SafeDate(year, monthNumber, 8),
                userId: userId,
                merchant: "PSE&G",
                paymentMethod: "Bank Transfer",
                expenseType: ExpenseType.Essential,
                isRecurring: true,
                notes: "Electric and gas bill");

            AddExpenseIfCategoryExists(
                expenses,
                categories,
                categoryName: "Utilities",
                amount: 79.99m,
                expenseDate: SafeDate(year, monthNumber, 12),
                userId: userId,
                merchant: "Verizon Fios",
                paymentMethod: "Credit Card",
                expenseType: ExpenseType.Essential,
                isRecurring: true,
                notes: "Internet bill");

            AddExpenseIfCategoryExists(
                expenses,
                categories,
                categoryName: "Insurance",
                amount: 118.50m,
                expenseDate: SafeDate(year, monthNumber, 15),
                userId: userId,
                merchant: "GEICO",
                paymentMethod: "Credit Card",
                expenseType: ExpenseType.Essential,
                isRecurring: true,
                notes: "Car insurance premium");

            AddExpenseIfCategoryExists(
                expenses,
                categories,
                categoryName: "Subscriptions",
                amount: 15.49m,
                expenseDate: SafeDate(year, monthNumber, 5),
                userId: userId,
                merchant: "Netflix",
                paymentMethod: "Credit Card",
                expenseType: ExpenseType.NonEssential,
                isRecurring: true,
                notes: "Netflix monthly plan");

            AddExpenseIfCategoryExists(
                expenses,
                categories,
                categoryName: "Subscriptions",
                amount: 10.99m,
                expenseDate: SafeDate(year, monthNumber, 18),
                userId: userId,
                merchant: "Spotify",
                paymentMethod: "Credit Card",
                expenseType: ExpenseType.NonEssential,
                isRecurring: true,
                notes: "Spotify monthly plan");
        }

        // Realistic variable expenses: frequent groceries and dining, occasional health,
        // shopping, education, travel, and personal care.
        var expenseTemplates = new[]
        {
            new ExpenseTemplate("Groceries", "Costco", 95m, 210m, ExpenseType.Essential, "Bulk groceries"),
            new ExpenseTemplate("Groceries", "Trader Joe's", 35m, 95m, ExpenseType.Essential, "Groceries"),
            new ExpenseTemplate("Groceries", "Walmart", 45m, 140m, ExpenseType.Essential, "Groceries and household supplies"),
            new ExpenseTemplate("Dining", "Starbucks", 5m, 18m, ExpenseType.NonEssential, "Coffee and snacks"),
            new ExpenseTemplate("Dining", "Chipotle", 12m, 35m, ExpenseType.NonEssential, "Lunch"),
            new ExpenseTemplate("Dining", "DoorDash", 24m, 65m, ExpenseType.NonEssential, "Dinner delivery"),
            new ExpenseTemplate("Transport", "NJ Transit", 8m, 42m, ExpenseType.Essential, "Train fare"),
            new ExpenseTemplate("Transportation", "NJ Transit", 8m, 42m, ExpenseType.Essential, "Train fare"),
            new ExpenseTemplate("Transport", "Shell", 35m, 75m, ExpenseType.Essential, "Gas refill"),
            new ExpenseTemplate("Transportation", "Shell", 35m, 75m, ExpenseType.Essential, "Gas refill"),
            new ExpenseTemplate("Health", "CVS Pharmacy", 12m, 85m, ExpenseType.Essential, "Pharmacy items"),
            new ExpenseTemplate("Healthcare", "CVS Pharmacy", 12m, 85m, ExpenseType.Essential, "Pharmacy items"),
            new ExpenseTemplate("Health", "CityMD", 75m, 180m, ExpenseType.Essential, "Urgent care copay"),
            new ExpenseTemplate("Healthcare", "CityMD", 75m, 180m, ExpenseType.Essential, "Urgent care copay"),
            new ExpenseTemplate("Shopping", "Amazon", 18m, 160m, ExpenseType.NonEssential, "Online shopping"),
            new ExpenseTemplate("Shopping", "Target", 25m, 130m, ExpenseType.NonEssential, "Home essentials"),
            new ExpenseTemplate("Personal Care", "Great Clips", 22m, 55m, ExpenseType.NonEssential, "Haircut"),
            new ExpenseTemplate("Personal Care", "Ulta Beauty", 18m, 95m, ExpenseType.NonEssential, "Beauty and skincare"),
            new ExpenseTemplate("Entertainment", "AMC Theatres", 18m, 60m, ExpenseType.NonEssential, "Movie tickets"),
            new ExpenseTemplate("Entertainment", "Bowlero", 35m, 90m, ExpenseType.NonEssential, "Bowling"),
            new ExpenseTemplate("Education", "Udemy", 12m, 35m, ExpenseType.NonEssential, "Online course purchase"),
            new ExpenseTemplate("Education", "LinkedIn Learning", 29.99m, 29.99m, ExpenseType.NonEssential, "Learning subscription"),
            new ExpenseTemplate("Travel", "United Airlines", 240m, 650m, ExpenseType.NonEssential, "Flight ticket"),
            new ExpenseTemplate("Travel", "Marriott", 180m, 420m, ExpenseType.NonEssential, "Hotel booking"),
            new ExpenseTemplate("Miscellaneous", "USPS", 5m, 28m, ExpenseType.NonEssential, "Postage")
        };

        for (var weekStart = sixMonthsAgo; weekStart <= today; weekStart = weekStart.AddDays(7))
        {
            AddRandomExpenseFromTemplate(expenses, categories, userId, faker, expenseTemplates, "Groceries", weekStart.AddDays(faker.Random.Int(0, 2)), paymentMethods);
            AddRandomExpenseFromTemplate(expenses, categories, userId, faker, expenseTemplates, "Dining", weekStart.AddDays(faker.Random.Int(1, 6)), paymentMethods);

            if (faker.Random.Bool(0.45f))
            {
                AddRandomExpenseFromTemplate(expenses, categories, userId, faker, expenseTemplates, "Shopping", weekStart.AddDays(faker.Random.Int(0, 6)), paymentMethods);
            }

            if (faker.Random.Bool(0.35f))
            {
                AddRandomExpenseFromTemplate(expenses, categories, userId, faker, expenseTemplates, HasCategory(categories, "Transport") ? "Transport" : "Transportation", weekStart.AddDays(faker.Random.Int(0, 6)), paymentMethods);
            }
        }

        for (var i = 0; i < 18; i++)
        {
            var categoryName = faker.PickRandom(new[]
            {
                HasCategory(categories, "Health") ? "Health" : "Healthcare",
                "Personal Care",
                "Entertainment",
                "Education",
                "Travel",
                "Miscellaneous"
            });

            AddRandomExpenseFromTemplate(
                expenses,
                categories,
                userId,
                faker,
                expenseTemplates,
                categoryName,
                faker.Date.Between(sixMonthsAgo, today),
                paymentMethods);
        }

        await context.Expenses.AddRangeAsync(expenses.OrderBy(x => x.ExpenseDate));
    }

    private static void AddRandomExpenseFromTemplate(
        List<Expense> expenses,
        List<Category> categories,
        string userId,
        Faker faker,
        ExpenseTemplate[] templates,
        string categoryName,
        DateTime expenseDate,
        string[] paymentMethods)
    {
        var availableTemplates = templates
            .Where(x => x.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (availableTemplates.Length == 0)
        {
            return;
        }

        var template = faker.PickRandom(availableTemplates);

        AddExpenseIfCategoryExists(
            expenses,
            categories,
            template.CategoryName,
            faker.Random.Decimal(template.MinAmount, template.MaxAmount),
            expenseDate,
            userId,
            template.Merchant,
            faker.PickRandom(paymentMethods),
            template.ExpenseType,
            isRecurring: false,
            notes: template.Notes);
    }

    private static void AddExpenseIfCategoryExists(
        List<Expense> expenses,
        List<Category> categories,
        string categoryName,
        decimal amount,
        DateTime expenseDate,
        string userId,
        string merchant,
        string paymentMethod,
        ExpenseType expenseType,
        bool isRecurring,
        string? notes)
    {
        if (expenseDate.Date > DateTime.Today)
        {
            return;
        }

        var category = categories.FirstOrDefault(x => x.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

        if (category is null)
        {
            return;
        }

        expenses.Add(new Expense(
            amount: Math.Round(amount, 2),
            expenseDate: expenseDate.Date,
            categoryId: category.Id,
            userId: userId,
            currency: "USD",
            notes: notes,
            merchant: merchant,
            paymentMethod: paymentMethod,
            expenseType: expenseType,
            isRecurring: isRecurring));
    }

    private static bool HasCategory(List<Category> categories, string categoryName)
    {
        return categories.Any(x => x.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
    }

    private static DateTime SafeDate(int year, int month, int day)
    {
        return new DateTime(year, month, Math.Min(day, DateTime.DaysInMonth(year, month)));
    }

    private sealed record ExpenseTemplate(
        string CategoryName,
        string Merchant,
        decimal MinAmount,
        decimal MaxAmount,
        ExpenseType ExpenseType,
        string Notes);

    private static async Task SeedIncomeAsync(
        ApplicationDbContext context,
        string userId)
    {
        var incomes = new List<Income>();

        for (var i = 5; i >= 0; i--)
        {
            var month = DateTime.Today.AddMonths(-i);
            var date = new DateTime(month.Year, month.Month, 1);

            incomes.Add(new Income(
                amount: 5200,
                receivedOn: date,
                source: "Salary",
                userId: userId,
                currency: "USD",
                notes: "Monthly salary deposit",
                isRecurring: true
            ));
        }

        incomes.Add(new Income(
            amount: 750,
            receivedOn: DateTime.Today.AddDays(-20),
            source: "Freelance Project",
            userId: userId,
            currency: "USD",
            notes: "Side income",
            isRecurring: false
        ));

        await context.Incomes.AddRangeAsync(incomes);
    }

    private static async Task SeedBudgetsAsync(
        ApplicationDbContext context,
        string userId,
        List<Category> categories)
    {
        var currentMonthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

        var groceryCategory = categories.FirstOrDefault(x => x.Name == "Groceries");
        var diningCategory = categories.FirstOrDefault(x => x.Name == "Dining");
        var entertainmentCategory = categories.FirstOrDefault(x => x.Name == "Entertainment");

        var budgets = new List<Budget>
        {
            new(
                name: "Monthly Overall Budget",
                limitAmount: 4500,
                period: BudgetPeriod.Monthly,
                startDate: currentMonthStart,
                endDate: currentMonthEnd,
                userId: userId
            )
        };

        if (groceryCategory is not null)
        {
            budgets.Add(new Budget(
                name: "Groceries Budget",
                limitAmount: 700,
                period: BudgetPeriod.Monthly,
                startDate: currentMonthStart,
                endDate: currentMonthEnd,
                userId: userId,
                categoryId: groceryCategory.Id
            ));
        }

        if (diningCategory is not null)
        {
            budgets.Add(new Budget(
                name: "Dining Budget",
                limitAmount: 400,
                period: BudgetPeriod.Monthly,
                startDate: currentMonthStart,
                endDate: currentMonthEnd,
                userId: userId,
                categoryId: diningCategory.Id
            ));
        }

        if (entertainmentCategory is not null)
        {
            budgets.Add(new Budget(
                name: "Entertainment Budget",
                limitAmount: 250,
                period: BudgetPeriod.Monthly,
                startDate: currentMonthStart,
                endDate: currentMonthEnd,
                userId: userId,
                categoryId: entertainmentCategory.Id
            ));
        }

        await context.Budgets.AddRangeAsync(budgets);
    }
}