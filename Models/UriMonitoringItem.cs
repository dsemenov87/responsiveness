namespace responsiveness.Models;

public sealed class UriMonitoringItem
{
    private readonly List<UriMeasurement> _measurements = new(1024);
    private readonly List<DateTimeOffset> _errors = new(1024);
    public IReadOnlyCollection<UriMeasurement> Measurements => _measurements;
    public void AddMeasurement(UriMeasurement measurement) => _measurements.Add(measurement);
    
    public required Uri Uri { get; init; }
    public Guid Uuid { get; } = Guid.NewGuid();

    public int TotalErrorCount => _errors.Count;
    public IReadOnlyCollection<DateTimeOffset> Errors => _errors; 
    public void IncrementErrors() => _errors.Add(DateTimeOffset.Now);
}