namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class VertexApiException(Error error)
    : InvalidOperationException(error.Message)
{
    public Error Error { get; } = error;
}