namespace CSharpApp.Api.Dtos;

public class RequestStatisticsDto
{
    public int TotalRequests { get; set; }
    public double AverageTimeMs { get; set; }
    public double MedianTimeMs { get; set; }
    public double MinTimeMs { get; set; }
    public double MaxTimeMs { get; set; }
    public List<EndpointStatisticsDto> Endpoints { get; set; } = new();
}
