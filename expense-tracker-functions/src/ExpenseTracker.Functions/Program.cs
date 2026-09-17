using ExpenseTracker.Infrastructure;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddExpenseExportInfrastructure(context.Configuration);
    })
    .Build();

host.Run();