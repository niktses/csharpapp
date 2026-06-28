using System.Diagnostics;
using CSharpApp.Api.Services;

namespace CSharpApp.Api.Middleware;

public class RequestPerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestPerformanceMiddleware> _logger;

    public RequestPerformanceMiddleware(RequestDelegate next, ILogger<RequestPerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IRequestMetricsService metricsService)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Skip swagger/openapi/internal paths to avoid pollution
        if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/internal", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var initialEndpoint = context.GetEndpoint();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
            var method = context.Request.Method;
            var statusCode = context.Response.StatusCode;

            // Try to get endpoint routing information (fallback to initial if cleared by exception handler)
            var endpoint = context.GetEndpoint() ?? initialEndpoint;
            var routePattern = (endpoint as RouteEndpoint)?.RoutePattern.RawText;

            // Log request performance
            _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs:F2} ms. Route: {RoutePattern}",
                method, path, statusCode, elapsedMs, routePattern ?? "N/A");

            // Record metrics
            metricsService.RecordRequest(path, method, routePattern, elapsedMs, statusCode);
        }
    }
}
