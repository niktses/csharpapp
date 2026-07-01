namespace CSharpApp.Api.Models;

public class RequestMetric
{
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string? RoutePattern { get; set; }
    public double ElapsedMilliseconds { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
}
