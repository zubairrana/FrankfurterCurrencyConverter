using System.Net;
using System.Text.Json;
using CurrencyConverter.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            RestrictedCurrencyException => (
                HttpStatusCode.BadRequest,
                "Restricted Currency",
                exception.Message),

            CurrencyNotFoundException => (
                HttpStatusCode.BadRequest,
                "Currency Not Found",
                exception.Message),

            ArgumentException => (
                HttpStatusCode.BadRequest,
                "Invalid Request",
                exception.Message),

            ExternalApiException ex when ex.StatusCode == HttpStatusCode.NotFound => (
                HttpStatusCode.NotFound,
                "Resource Not Found",
                "The requested resource was not found in the external provider."),

            ExternalApiException ex when ex.StatusCode == HttpStatusCode.UnprocessableContent => (
                HttpStatusCode.BadRequest,
                "Currency Not Found",
                exception.Message),

            ExternalApiException => (
                HttpStatusCode.ServiceUnavailable,
                "External Provider Unavailable",
                "External currency provider is currently unavailable. Please try again later."),
            
            _ => (
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.")
        };

        var problemDetails = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{(int)statusCode}",
            Title = title,
            Status = (int)statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        // Add traceId as extension — standard practice
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.ContentType = "application/problem+json"; // RFC 7807 content type
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }
}