namespace responsiveness.Abstractions;

public sealed record UriMeasurement(DateTimeOffset Timestamp, IReadOnlyCollection<UriMetrics> MetricsList);