using CSharpApp.Api.Dtos;

namespace CSharpApp.Api.Services;

public interface IRequestMetricsService
{
    void RecordRequest(string path, string method, string? routePattern, double elapsedMilliseconds, int statusCode);
    RequestStatisticsDto GetStatistics();
}
