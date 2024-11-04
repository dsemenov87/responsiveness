using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using responsiveness.Abstractions;

namespace responsiveness.Benchmark;

public sealed class UriBenchmarkService(
    IHttpClientFactory httpClientFactory,
    IStatsCalculator statsCalculator,
    IOptions<UriBenchmarkOptions> options,
    ILogger<UriBenchmarkService> logger
    ): IUriBenchmarkService
{
    public async Task<UriMeasurement?> RunAsync(Uri uri, CancellationToken cancellation = default)
    {
        var data = new Dictionary<HttpRequestStage, List<double>>();
        for (var num = 0; num < options.Value.BenchmarkRepeats; num++)
        {
            var timings = await MakeRequestAsync(uri, cancellation);
            if (timings is null)
                return null;

            foreach (var (stage, time) in timings)
            {
                if (data.TryGetValue(stage, out var value))
                    value.Add(time);
                else
                {
                    var newList = new List<double>(64) { time };
                    data.Add(stage, newList);
                }
            }
            
            await Task.Delay(options.Value.BenchmarkDelayMs, cancellation);
        }

        var metrics = data.Select(kv => 
        {
            var mean = statsCalculator.Mean(kv.Value);
            var stdDev = statsCalculator.StdDev(kv.Value, mean);
            var median = statsCalculator.Median(kv.Value);
            return new UriMetrics(kv.Key, mean, stdDev, median);
        })
        .ToArray();

        return new UriMeasurement(DateTimeOffset.Now, metrics);
    }

    private async Task<HttpRequestTimings?> MakeRequestAsync(Uri uri, CancellationToken cancellation = default)
    {
        try
        {
            using var httpClient = httpClientFactory.CreateClient(nameof(UriBenchmarkService));
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
        catch (Exception ex)
        {
            if (ex is not HttpRequestException and not TimeoutException)
                throw;
            logger.LogWarning("Error while sending request to url {Uri}: {ErrorMessage}", uri, ex.Message);
            return null;
        }
    }
}