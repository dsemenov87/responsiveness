namespace responsiveness.Abstractions;

/// <summary>
/// URI measurement result
/// </summary>
/// <param name="Mean">Arithmetic mean of all measurements, ms</param>
/// <param name="StdDev">Standard deviation of all measurements, ms</param>
/// <param name="Median">Value separating the higher half of all measurements (50th percentile), ms</param>
public sealed record UriMetrics(HttpRequestStage Stage, double Mean, double StdDev, double Median);