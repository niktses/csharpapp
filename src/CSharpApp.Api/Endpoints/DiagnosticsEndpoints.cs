using CSharpApp.Api.Dtos;
using CSharpApp.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CSharpApp.Api.Endpoints;

public static class DiagnosticsEndpoints
{
    public static IEndpointRouteBuilder MapDiagnosticsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("internal/statistics", (IRequestMetricsService metricsService) =>
        {
            var statistics = metricsService.GetStatistics();
            return Results.Ok(statistics);
        })
        .WithName("GetInternalStatistics")
        .WithGroupName("internal");

        return builder;
    }
}
