namespace responsiveness.Models;

public sealed class UriMonitoringProcess: IDisposable
{
    private readonly List<UriMeasurement> _measurements = new(1024);
    private readonly List<DateTimeOffset> _errors = new(1024);
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _task;
    
    public UriMonitoringProcess(Uri uri, Func<Action<UriMeasurement>, Action, CancellationToken, Task> taskCtor)
    {
        Uri = uri;
        _task = taskCtor(AddMeasurement, IncrementErrors, _cts.Token);
        return;
        void AddMeasurement(UriMeasurement measurement) => _measurements.Add(measurement);
        void IncrementErrors() => _errors.Add(DateTimeOffset.Now);
    }
    
    public IReadOnlyCollection<UriMeasurement> Measurements => _measurements;

    public UriStatus Status { get; private set; } = UriStatus.Alive;
    public Uri Uri { get; }
    public int TotalErrorCount => _errors.Count;
    public IReadOnlyCollection<DateTimeOffset> Errors => _errors;

    public void Dispose()
    {
        if (_cts.Token.CanBeCanceled)
        {
            _cts?.Cancel();
        }
        _cts?.Dispose();
    }
}

public enum UriStatus
{
    Alive,
    Dead,
}