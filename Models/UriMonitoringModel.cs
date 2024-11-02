using System.Collections;
using System.Collections.Concurrent;

namespace responsiveness.Models;

public sealed class UriMonitoringModel: IEnumerable<UriMonitoringItem>
{
    private readonly ConcurrentDictionary<Uri, UriMonitoringItem> _items = new();

    public event Action<Uri>? UriAdded;
    public event Action<Uri>? UriRemoved;
   
    public DateTimeOffset StartTime { get; } = DateTimeOffset.UtcNow;
    public IReadOnlyCollection<Uri> Uris => _items.Keys.ToArray();
    
    public UriMonitoringItem this[Guid uuid] =>
        _items.Values.FirstOrDefault(x => x.Uuid == uuid)
            ?? throw new KeyNotFoundException();
    
    public void AddUri(Uri uri)
    {
        if (_items.TryAdd(uri, new UriMonitoringItem { Uri = uri }))
        {
            UriAdded?.Invoke(uri);
        };
    }
    
    public void RemoveUri(Uri uri)
    {
        _items.TryRemove(uri, out var _);
        UriRemoved?.Invoke(uri);
    }
    
    public IEnumerator<UriMonitoringItem> GetEnumerator() => _items.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void UpdateErrors(Uri uri)
    {
        _items.AddOrUpdate(uri,
            (_) =>
            {
                var item = new UriMonitoringItem { Uri = uri };
                item.IncrementErrors();
                return item;
            },
            (_, item) =>
            {
                item.IncrementErrors();
                return item;
            });
    }
    
    public void UpdateStatistics(Uri uri, UriMeasurement measurement)
    {
        _items.AddOrUpdate(uri,
            (_) =>
            {
                var item = new UriMonitoringItem { Uri = uri };
                item.AddMeasurement(measurement);
                return item;
            },
            (_, item) =>
            {
                item.AddMeasurement(measurement);
                return item;
            });
    }
}