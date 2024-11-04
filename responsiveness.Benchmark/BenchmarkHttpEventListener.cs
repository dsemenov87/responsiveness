using System.Diagnostics.Tracing;
using responsiveness.Abstractions;

namespace responsiveness.Benchmark;

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

        // regarding your concern that with this approach you might receive events
        // not related to the specific web request you are interested in. You can do the
        // correlation using AsyncLocal variable, as documentation mentions here.
        // The idea is simple - you use AsyncLocal variable and set its value to something
        // (such as class holding your timing information) before doing request with HttpClient.
        // Then you perform request. Now, when new event comes in - you check the value of
        // AsyncLocal variable. If it's not null - then this event is related to the current
        // request, otherwise you can ignore it.
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
        var timings = new HttpRequestTimings();
        
        timings.Put(HttpRequestStage.Request, raw.RequestStop - raw.RequestStart);
        timings.Put(HttpRequestStage.Dns, raw.DnsStop - raw.DnsStart);
        timings.Put(HttpRequestStage.SslHandshake, raw.SslHandshakeStop - raw.SslHandshakeStart);
        timings.Put(HttpRequestStage.SocketConnect, raw.SocketConnectStop - raw.SocketConnectStart);
        timings.Put(HttpRequestStage.RequestHeaders, raw.RequestHeadersStop - raw.RequestHeadersStart);
        timings.Put(HttpRequestStage.ResponseHeaders, raw.ResponseHeadersStop - raw.ResponseHeadersStart);
        timings.Put(HttpRequestStage.ResponseContent, raw.ResponseContentStop - raw.ResponseContentStart);
        
        return timings;
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