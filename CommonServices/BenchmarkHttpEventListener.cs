using System.Diagnostics.Tracing;

namespace responsiveness.CommonServices;

/// <summary>
/// https://stackoverflow.com/questions/74884835/c-sharp-httpclient-response-time-break-down
/// </summary>
internal sealed class BenchmarkHttpEventListener : EventListener
{
    // Constant necessary for attaching ActivityId to the events.
    private const EventKeywords TasksFlowActivityIds = (EventKeywords)0x80;
    private readonly AsyncLocal<HttpRequestTimingDataRaw> _timings = new();

    internal BenchmarkHttpEventListener()
    {
        // set variable here
        _timings.Value = new HttpRequestTimingDataRaw();
    }

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        switch (eventSource.Name)
        {
            // List of event source names provided by networking in .NET 5.
            case "System.Net.Http":
            case "System.Net.Sockets":
            case "System.Net.Security":
            case "System.Net.NameResolution":
                EnableEvents(eventSource, EventLevel.LogAlways);
                break;
            // Turn on ActivityId.
            case "System.Threading.Tasks.TplEventSource":
                // Attach ActivityId to the events.
                EnableEvents(eventSource, EventLevel.LogAlways, TasksFlowActivityIds);
                break;
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        var timings = _timings.Value;
        if (timings is null)
            return; // some event which is not related to this scope, ignore it
        var fullName = $"{eventData.EventSource.Name}.{eventData.EventName}";

        switch (fullName)
        {
            case "System.Net.Http.RequestStart":
                timings.RequestStart = eventData.TimeStamp;
                break;
            case "System.Net.Http.RequestStop":
                timings.RequestStop = eventData.TimeStamp;
                break;
            case "System.Net.NameResolution.ResolutionStart":
                timings.DnsStart = eventData.TimeStamp;
                break;
            case "System.Net.NameResolution.ResolutionStop":
                timings.DnsStop = eventData.TimeStamp;
                break;
            case "System.Net.Sockets.ConnectStart":
                timings.SocketConnectStart = eventData.TimeStamp;
                break;
            case "System.Net.Sockets.ConnectStop":
                timings.SocketConnectStop = eventData.TimeStamp;
                break;
            case "System.Net.Security.HandshakeStart":
                timings.SslHandshakeStart = eventData.TimeStamp;
                break;
            case "System.Net.Security.HandshakeStop":
                timings.SslHandshakeStop = eventData.TimeStamp;
                break;
            case "System.Net.Http.RequestHeadersStart":
                timings.RequestHeadersStart = eventData.TimeStamp;
                break;
            case "System.Net.Http.RequestHeadersStop":
                timings.RequestHeadersStop = eventData.TimeStamp;
                break;
            case "System.Net.Http.ResponseHeadersStart":
                timings.ResponseHeadersStart = eventData.TimeStamp;
                break;
            case "System.Net.Http.ResponseHeadersStop":
                timings.ResponseHeadersStop = eventData.TimeStamp;
                break;
            case "System.Net.Http.ResponseContentStart":
                timings.ResponseContentStart = eventData.TimeStamp;
                break;
            case "System.Net.Http.ResponseContentStop":
                timings.ResponseContentStop = eventData.TimeStamp;
                break;
        }
    }

    public HttpRequestTimings GetMeasurement()
    {
        var raw = _timings.Value!;
        return new()
        {
            Request = raw.RequestStop - raw.RequestStart,
            Dns = raw.DnsStop - raw.DnsStart,
            SslHandshake = raw.SslHandshakeStop - raw.SslHandshakeStart,
            SocketConnect = raw.SocketConnectStop - raw.SocketConnectStart,
            RequestHeaders = raw.RequestHeadersStop - raw.RequestHeadersStart,
            ResponseHeaders = raw.ResponseHeadersStop - raw.ResponseHeadersStart,
            ResponseContent = raw.ResponseContentStop - raw.ResponseContentStart
        };
    }


    private class HttpRequestTimingDataRaw
    {
        public DateTime? DnsStart { get; set; }
        public DateTime? DnsStop { get; set; }
        public DateTime? RequestStart { get; set; }
        public DateTime? RequestStop { get; set; }
        public DateTime? SocketConnectStart { get; set; }
        public DateTime? SocketConnectStop { get; set; }
        public DateTime? SslHandshakeStart { get; set; }
        public DateTime? SslHandshakeStop { get; set; }
        public DateTime? RequestHeadersStart { get; set; }
        public DateTime? RequestHeadersStop { get; set; }
        public DateTime? ResponseHeadersStart { get; set; }
        public DateTime? ResponseHeadersStop { get; set; }
        public DateTime? ResponseContentStart { get; set; }
        public DateTime? ResponseContentStop { get; set; }
    }
}