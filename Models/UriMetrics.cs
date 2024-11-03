using responsiveness.CommonServices;

namespace responsiveness.Models;

/// <summary>
/// URI measurement result
/// </summary>
/// <param name="Mean">Arithmetic mean of all measurements, us - 1 Microsecond (0.000001 sec)</param>
/// <param name="StdDev">Standard deviation of all measurements, us - 1 Microsecond (0.000001 sec)</param>
/// <param name="Median">Value separating the higher half of all measurements (50th percentile), us - 1 Microsecond (0.000001 sec)</param>
public sealed record UriMetrics(HttpRequestStage Stage, double Mean, double StdDev, double Median);