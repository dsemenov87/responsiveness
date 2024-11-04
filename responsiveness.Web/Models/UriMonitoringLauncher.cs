using Microsoft.Extensions.Options;
using responsiveness.CommonServices;

namespace responsiveness.Models;

public sealed class UriMonitoringLauncher(
    IOptions<UriMonitoringOptions> options,
    IUriBenchmarkService uriBenchmarkService,
    ILogger<UriMonitoringLauncher> logger
) : IUriMonitoringLauncher
{
    public UriMonitoringProcess LaunchProcess(Uri uri) =>
        new(uri, (os, oe, ct) => LoopAsync(uri, os, oe, ct));
    
    private async Task LoopAsync(Uri uri, Action<UriMeasurement> onSuccess, Action onError, CancellationToken cancellation)
    {
        logger.LogInformation("Start monitoring {Uri}", uri);
        
        var delayTime = TimeSpan.FromSeconds(options.Value.DelayTimeSec);
        try
        {
            while (true)
            {
                var measurement = await uriBenchmarkService.RunAsync(uri, cancellation);

                if (measurement is not null)
                    onSuccess(measurement);
                else
                    onError();

                await Task.Delay(delayTime, cancellation);
            }
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Stop monitoring {Uri}", uri);
            throw;
        }
    }
}