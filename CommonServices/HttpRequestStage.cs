namespace responsiveness.CommonServices;

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