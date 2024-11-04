namespace responsiveness.Models;

public sealed record UriMeasurement(DateTimeOffset Timestamp, IReadOnlyCollection<UriMetrics> MetricsList);