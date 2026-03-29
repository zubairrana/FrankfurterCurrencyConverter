using CurrencyConverter.API.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Threading.RateLimiting;

namespace CurrencyConverter.API.Extensions
{
    public static class ApiRateLimitingExtensions
    {
        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = 429;

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType = "application/problem+json";
                    await context.HttpContext.Response.WriteAsync(
                        System.Text.Json.JsonSerializer.Serialize(new ProblemDetails
                        {
                            Type = "https://httpstatuses.com/429",
                            Title = "Too Many Requests",
                            Status = 429,
                            Detail = "You have exceeded the allowed number of requests. Please try again later.",
                            Instance = context.HttpContext.Request.Path
                        }), cancellationToken);
                };

                // Global fixed window — 60 requests per minute per IP
                options.AddPolicy(RateLimitPolicies.Global, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 60,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

                // Stricter limit for auth endpoint
                options.AddPolicy(RateLimitPolicies.Auth, context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
            });

            return services;
        }
    }
}
