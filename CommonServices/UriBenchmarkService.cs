using Microsoft.Extensions.Options;
using responsiveness.Models;

namespace responsiveness.CommonServices;

public sealed class UriBenchmarkService(
    IHttpClientFactory httpClientFactory,
    IStatsCalculator statsCalculator,
    IOptions<UriMonitoringOptions> options,
    ILogger<UriBenchmarkService> logger
    ): IUriBenchmarkService
{
    public async Task<UriMeasurement?> RunAsync(Uri uri, CancellationToken cancellation = default)
    {
        var data = new double[options.Value.BenchmarkRepeats];
        for (var num = 0; num < options.Value.BenchmarkRepeats; num++)
        {
            var timings = await MakeRequestAsync(uri, cancellation);
            if (timings?.Request.HasValue != true)
                return null;
            
            data[num] = timings.Request.Value.Microseconds;
            
            await Task.Delay(options.Value.BenchmarkDelayMs, cancellation);
        }

        var mean = statsCalculator.CalculateMean(data);
        var stdDev = statsCalculator.StandartDeviation(data, mean);
        var median = statsCalculator.Median(data);
        
        return new UriMeasurement(DateTimeOffset.Now, mean, stdDev, median);
    }

    private async Task<HttpRequestTimings?> MakeRequestAsync(Uri uri, CancellationToken cancellation = default)
    {
        try
        {
            using var httpClient = httpClientFactory.CreateClient($"{uri}");
            using var listener = new BenchmarkHttpEventListener();
            // we start new listener scope here
            // only this specific request timings will be measured
            // this implementation assumes usage of exactly one BenchmarkHttpEventListener per request
            using var response = await httpClient.GetAsync(uri, cancellation);
            if (response.IsSuccessStatusCode)
            {
                return listener.GetMeasurement();
            }

            logger.LogWarning("Error while sending request to url {Uri}: {StatusCode}", uri, response.StatusCode);
            return null;
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning("Error while sending request to url {Uri}: {ErrorMessage}", uri, ex.Message);
            return null;
        }
    }
}