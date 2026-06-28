namespace CSharpApp.Api.Dtos;

public class EndpointStatisticsDto
{
    public string Method { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public double AverageTimeMs { get; set; }
    public double MedianTimeMs { get; set; }
    public double MinTimeMs { get; set; }
    public double MaxTimeMs { get; set; }
}
