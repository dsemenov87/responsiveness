namespace responsiveness.Components.Models;

public class LinePlotData
{
    public required string MetricName { get; init; }
    public required double Timestamp { get; init; }
    public required double N { get; init; }
}