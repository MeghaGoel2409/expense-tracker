using Asp.Versioning.ApiExplorer;
using ExpenseTracker.WebApi.Middleware;
using Serilog;

public static class MiddlewareExtensions
{
    public static WebApplication UseWebApiPipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseCorrelationId();
        app.UseGlobalExceptionHandling();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors("Frontend");
        app.UseAuthentication();
        app.UseUserContext();
        app.UseAuthorization();

        app.MapControllers();
        return app;
    }
}