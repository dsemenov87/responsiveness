using System.Text;

namespace responsiveness.CommonServices;

public sealed class HttpRequestTimings
{
    public TimeSpan? Request { get; init; }
    public TimeSpan? Dns { get; init; }
    public TimeSpan? SslHandshake { get; init; }
    public TimeSpan? SocketConnect { get; init; }
    public TimeSpan? RequestHeaders { get; init; }
    public TimeSpan? ResponseHeaders { get; init; }
    public TimeSpan? ResponseContent { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (Request != null)
            sb.AppendLine($"Total time: {Request.Value.TotalMilliseconds:N0}ms");
        if (Dns != null)
            sb.AppendLine($"DNS: {Dns.Value.TotalMilliseconds:N0}ms");
        if (SocketConnect != null)
            sb.AppendLine($"Socket connect: {SocketConnect.Value.TotalMilliseconds:N0}ms");
        if (SslHandshake != null)
            sb.AppendLine($"SSL Handshake: {SslHandshake.Value.TotalMilliseconds:N0}ms");
        if (RequestHeaders != null)
            sb.AppendLine($"Request headers: {RequestHeaders.Value.TotalMilliseconds:N0}ms");
        if (ResponseHeaders != null)
            sb.AppendLine($"Response headers: {ResponseHeaders.Value.TotalMilliseconds:N0}ms");
        if (ResponseContent != null)
            sb.AppendLine($"Response content: {ResponseContent.Value.TotalMilliseconds:N0}ms");

        return sb.ToString();
    }
}