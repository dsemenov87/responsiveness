using System.Collections.Concurrent;
using responsiveness.Web.Models;

namespace responsiveness.Models;

public sealed class UriMonitoringModel
{
    private readonly ConcurrentDictionary<Uri, UriMonitoringProcess> _processes = new();
    public IReadOnlyCollection<UriMonitoringProcess> GetProcesses() => _processes.Values.ToArray();
    
    public DateTimeOffset StartTime { get; } = DateTimeOffset.UtcNow;
    
    public UriMonitoringProcess this[Uri uri] =>
        _processes.TryGetValue(uri, out var process)
            ? process 
            : throw new KeyNotFoundException();
    
    public void AddUriMonitoring(Uri uri, UriMonitoringProcess process)
    {
        _processes.TryAdd(uri, process);
    }
    
    public void StopUriMonitoring(Uri uri)
    {
        if (_processes.TryRemove(uri, out var process))
        {
            process.Dispose();    
        }
    }
}