using responsiveness.Abstractions;

namespace responsiveness.Web.Models;

public sealed class UriMonitoringProcess: IDisposable
{
    private readonly Func<Action<UriMeasurement>, Action, CancellationToken, Task> _taskCtor;
    private readonly List<UriMeasurement> _measurements = new(1024);
    private readonly List<DateTimeOffset> _errors = new(1024);
    
    private CancellationTokenSource _cts = new();
    private Task _task;
    private int _repeatErrorCount = 0;
    
    public UriMonitoringProcess(Uri uri, Func<Action<UriMeasurement>, Action, CancellationToken, Task> taskCtor)
    {
        _taskCtor = taskCtor;
        Uri = uri;
        _task = StartTask();
    }
    
    public IReadOnlyCollection<UriMeasurement> Measurements => _measurements;

    public UriMonitoringStatus Status { get; private set; } = UriMonitoringStatus.Alive;
    public Uri Uri { get; }
    public int TotalErrorCount => _errors.Count;
    public IReadOnlyCollection<DateTimeOffset> Errors => _errors;

    public void EnsureHealthy()
    {
        if (Status == UriMonitoringStatus.Stopped || !_task.IsCompleted)
            return;

        _task = StartTask();
    }

    public void Pause()
    {
        StopTask();
        Status = UriMonitoringStatus.Stopped;
    }

    public void Resume()
    {
        _task = StartTask();
        Status = UriMonitoringStatus.Alive;
    }
    
    public void Dispose() => StopTask();

    private void StopTask()
    {
        if (_cts.Token.CanBeCanceled)
            _cts?.Cancel();
        _cts?.Dispose();
    }
    
    private Task StartTask()
    {
        _cts = new CancellationTokenSource();
        return _taskCtor(AddMeasurement, IncrementErrors, _cts.Token);
    }

    private void AddMeasurement(UriMeasurement measurement)
    {
        _measurements.Add(measurement);
        _repeatErrorCount = 0;
        Status = UriMonitoringStatus.Alive;
    }

    private void IncrementErrors()
    {
        _errors.Add(DateTimeOffset.Now);
        if (++_repeatErrorCount > 10)
        {
            Status = UriMonitoringStatus.DoesntResponse;
        }
    }
}