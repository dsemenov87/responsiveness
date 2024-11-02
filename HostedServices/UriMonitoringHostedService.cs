using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using responsiveness.CommonServices;
using responsiveness.Models;

namespace responsiveness.HostedServices;

public sealed class UriMonitoringHostedService(
    UriMonitoringModel model,
    IOptions<UriMonitoringOptions> options,
    IUriBenchmarkService uriBenchmarkService,
    ILogger<UriMonitoringHostedService> logger): IHostedService
{
    private readonly ConcurrentDictionary<Uri, CancellationTokenSource> _processes = new();
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Hosted service {ServiceName} was started", nameof(UriMonitoringHostedService));

        model.UriAdded += StartMonitoring;
        model.UriRemoved += StopMonitoring;
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Hosted service {ServiceName} was stopped", nameof(UriMonitoringHostedService));
        
        model.UriAdded -= StartMonitoring;
        model.UriRemoved -= StopMonitoring;

        foreach (var uri in _processes.Keys)
        {
            _processes[uri]?.Cancel();
        }
        
        return Task.CompletedTask;
    }

    private void StartMonitoring(Uri uri)
    {
        _processes.AddOrUpdate(uri, AddTask, UpdateTask);
        return;

        CancellationTokenSource AddTask(Uri uri1)
        {
            logger.LogInformation("Start monitoring {Uri}", uri1);
            
            var cts = new CancellationTokenSource();
            var _ = LoopAsync(uri1, cts.Token);
            return cts;
        }
        
        CancellationTokenSource UpdateTask(Uri uri1, CancellationTokenSource oldCts)
        {
            logger.LogWarning("Task for uri {Uri} already exists", uri1);
            
            oldCts?.Cancel();
            var cts = new CancellationTokenSource();
            var _ = LoopAsync(uri1, cts.Token);
            return cts;
        }
    }

    private void StopMonitoring(Uri uri)
    {
        logger.LogInformation("Stop monitoring {Uri}", uri);
        
        if (_processes.TryRemove(uri, out var cts))
        {
            cts?.Cancel();
        }
        else
        {
            logger.LogWarning("Can not find a monitoring task for {Uri}", uri);
        }
    }
    
    private async Task LoopAsync(Uri uri, CancellationToken cancellation)
    {
        var delayTime = TimeSpan.FromSeconds(options.Value.DelayTimeSec);
        
        while (!cancellation.IsCancellationRequested)
        {
            var measurement = await uriBenchmarkService.RunAsync(uri, cancellation);
            
            if (measurement is not null)
                model.UpdateStatistics(uri, measurement);
            else
                model.UpdateErrors(uri);

            await Task.Delay(delayTime, cancellation);
        }
    }
}