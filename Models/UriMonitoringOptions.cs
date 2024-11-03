using System.Collections;
using Microsoft.Extensions.Http.Resilience;

namespace responsiveness.Models;

public class UriMonitoringOptions
{
    public const string SectionName = "UriMonitoring";
    
    /// <summary>
    /// delay time, seconds
    /// </summary>
    public int DelayTimeSec { get; set; }

    /// <summary>
    /// number of benchmark repeats
    /// </summary>
    public int BenchmarkRepeats { get; set; }

    /// <summary>
    /// benchmark delay in milliseconds
    /// </summary>
    public int BenchmarkDelayMs { get; set; }

    /// <summary>
    /// HTTP client timeout in seconds
    /// </summary>
    public int HttpClientTimeoutSec { get; set; }
}