using System.Text.Json;
using CSharpApp.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        switch (exception)
        {
            case DownstreamApiException downstreamApiException:
                problemDetails.Title = "Downstream API Error";
                problemDetails.Status = (int)downstreamApiException.StatusCode;
                
                try
                {
                    var parsedJson = JsonSerializer.Deserialize<object>(downstreamApiException.Content);
                    problemDetails.Extensions["downstreamError"] = parsedJson;
                }
                catch
                {
                    problemDetails.Extensions["downstreamError"] = downstreamApiException.Content;
                }
                problemDetails.Detail = "The downstream API returned an error: " + downstreamApiException.Message;
                break;

            case HttpRequestException httpRequestException:
                problemDetails.Title = "Network Error";
                problemDetails.Status = httpRequestException.StatusCode.HasValue 
                    ? (int)httpRequestException.StatusCode.Value 
                    : StatusCodes.Status502BadGateway;
                problemDetails.Detail = "Failed to communicate with the downstream service: " + httpRequestException.Message;
                break;

            case TaskCanceledException:
                problemDetails.Title = "Gateway Timeout";
                problemDetails.Status = StatusCodes.Status504GatewayTimeout;
                problemDetails.Detail = "The request to the downstream service timed out.";
                break;

            case TimeoutException:
                problemDetails.Title = "Gateway Timeout";
                problemDetails.Status = StatusCodes.Status504GatewayTimeout;
                problemDetails.Detail = "The request to the downstream service timed out.";
                break;

            case JsonException:
                problemDetails.Title = "Invalid Downstream Response";
                problemDetails.Status = StatusCodes.Status502BadGateway;
                problemDetails.Detail = "Failed to parse the response from the downstream service.";
                break;

            default:
                problemDetails.Title = "Internal Server Error";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Detail = "An unexpected error occurred.";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
