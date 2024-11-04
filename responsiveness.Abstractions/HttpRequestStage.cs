namespace responsiveness.Abstractions;

public enum HttpRequestStage
{
    Dns,
    SocketConnect,
    SslHandshake,
    RequestHeaders,
    ResponseHeaders,
    ResponseContent,
    Request,
}