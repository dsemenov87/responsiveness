using responsiveness.Models;

namespace responsiveness.CommonServices;

public interface IUriBenchmarkService
{
    /// <summary>
    /// Make responsiveness benchmark for a certain URI 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="cancellation"></param>
    /// <returns>measurement or null if URI doesn't response</returns>
    Task<UriMeasurement?> RunAsync(Uri uri, CancellationToken cancellation = default);
}