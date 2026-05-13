using System.Text.Json;
using ExpenseTracker.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.WebApi.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Request was cancelled by the client. TraceId: {TraceId}",
                context.TraceIdentifier);
        }
        catch (DbUpdateException ex)
        {
            await HandleExceptionAsync(
                context,
                ex,
                StatusCodes.Status500InternalServerError,
                Error.Database("Database.Error", "A database error occurred while processing the request."));
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(
                context,
                ex,
                StatusCodes.Status500InternalServerError,
                Error.Failure("Server.Error", "An unexpected error occurred."));
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        int statusCode,
        Error error)
    {
        _logger.LogError(
            exception,
            "Unhandled exception occurred. TraceId: {TraceId}",
            context.TraceIdentifier);

        if (context.Response.HasStarted)
        {
            throw exception;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errors = new List<Error> { error };

        if (_environment.IsDevelopment())
        {
            errors.Add(Error.Failure("Server.ExceptionDetails", exception.Message));
        }

        var result = Result.Failure(errors);
        result.TraceId = context.TraceIdentifier;

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}