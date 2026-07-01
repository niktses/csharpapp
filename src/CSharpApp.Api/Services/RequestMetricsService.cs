using System.Collections.Concurrent;
using CSharpApp.Api.Dtos;
using CSharpApp.Api.Models;

namespace CSharpApp.Api.Services;

public class RequestMetricsService : IRequestMetricsService
{
    private readonly ConcurrentQueue<RequestMetric> _metrics = new();

    public void RecordRequest(string path, string method, string? routePattern, double elapsedMilliseconds, int statusCode)
    {
        _metrics.Enqueue(new RequestMetric
        {
            Path = path,
            Method = method,
            RoutePattern = routePattern,
            ElapsedMilliseconds = elapsedMilliseconds,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        });
    }

    public RequestStatisticsDto GetStatistics()
    {
        var metricsList = _metrics.ToList();
        var dto = new RequestStatisticsDto();
        
        if (metricsList.Count == 0)
        {
            return dto;
        }

        var allTimes = metricsList.Select(m => m.ElapsedMilliseconds).ToList();
        dto.TotalRequests = metricsList.Count;
        dto.AverageTimeMs = Math.Round(allTimes.Average(), 2);
        dto.MedianTimeMs = Math.Round(CalculateMedian(allTimes), 2);
        dto.MinTimeMs = Math.Round(allTimes.Min(), 2);
        dto.MaxTimeMs = Math.Round(allTimes.Max(), 2);

        var grouped = metricsList.GroupBy(m => new 
        { 
            m.Method, 
            Route = string.IsNullOrEmpty(m.RoutePattern) ? m.Path : m.RoutePattern 
        });

        foreach (var group in grouped)
        {
            var groupTimes = group.Select(m => m.ElapsedMilliseconds).ToList();
            dto.Endpoints.Add(new EndpointStatisticsDto
            {
                Method = group.Key.Method,
                Route = group.Key.Route,
                TotalRequests = group.Count(),
                AverageTimeMs = Math.Round(groupTimes.Average(), 2),
                MedianTimeMs = Math.Round(CalculateMedian(groupTimes), 2),
                MinTimeMs = Math.Round(groupTimes.Min(), 2),
                MaxTimeMs = Math.Round(groupTimes.Max(), 2)
            });
        }

        return dto;
    }

    private static double CalculateMedian(List<double> values)
    {
        if (values.Count == 0) return 0;
        var sorted = values.OrderBy(v => v).ToList();
        int count = sorted.Count;
        if (count % 2 == 1)
        {
            return sorted[count / 2];
        }
        else
        {
            return (sorted[(count / 2) - 1] + sorted[count / 2]) / 2.0;
        }
    }
}
