using System.Collections;
using System.Text;

namespace responsiveness.CommonServices;

public sealed class HttpRequestTimings: IEnumerable<(HttpRequestStage, double)>
{
    private readonly Dictionary<HttpRequestStage, double> _data = new();

    public double this[HttpRequestStage stage] => _data[stage];

    public void Put(HttpRequestStage stage, TimeSpan? timeSpan)
    {
        if (timeSpan.HasValue)
        {
            _data[stage] = timeSpan.Value.TotalMilliseconds;
        }
    }

    // public override string ToString()
    // {
    //     var sb = new StringBuilder();
    //     if (Request != null)
    //         sb.AppendLine($"Total time: {Request.Value.TotalMilliseconds:N0}ms");
    //     if (Dns != null)
    //         sb.AppendLine($"DNS: {Dns.Value.TotalMilliseconds:N0}ms");
    //     if (SocketConnect != null)
    //         sb.AppendLine($"Socket connect: {SocketConnect.Value.TotalMilliseconds:N0}ms");
    //     if (SslHandshake != null)
    //         sb.AppendLine($"SSL Handshake: {SslHandshake.Value.TotalMilliseconds:N0}ms");
    //     if (RequestHeaders != null)
    //         sb.AppendLine($"Request headers: {RequestHeaders.Value.TotalMilliseconds:N0}ms");
    //     if (ResponseHeaders != null)
    //         sb.AppendLine($"Response headers: {ResponseHeaders.Value.TotalMilliseconds:N0}ms");
    //     if (ResponseContent != null)
    //         sb.AppendLine($"Response content: {ResponseContent.Value.TotalMilliseconds:N0}ms");
    //
    //     return sb.ToString();
    // }
    public IEnumerator<(HttpRequestStage, double)> GetEnumerator() =>
        _data.Select(kv => (kv.Key, kv.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() 
    {
        return GetEnumerator();
    }
}