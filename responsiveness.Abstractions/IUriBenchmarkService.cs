namespace responsiveness.Abstractions;

public interface IUriBenchmarkService
{
    /// <summary>
    /// Make responsiveness benchmark for a certain URI 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="cancellation"></param>
    Task<UriMeasurement?> RunAsync(Uri uri, CancellationToken cancellation = default);
}