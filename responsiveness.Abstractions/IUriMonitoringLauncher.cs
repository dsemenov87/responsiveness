namespace responsiveness.Abstractions;

public interface IUriMonitoringLauncher<out TMonitoringProcess>
{
    TMonitoringProcess LaunchProcess(Uri uri);
}