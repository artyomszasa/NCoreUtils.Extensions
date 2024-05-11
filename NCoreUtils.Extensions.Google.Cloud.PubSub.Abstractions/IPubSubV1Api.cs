namespace NCoreUtils.Google.Cloud.PubSub;

public interface IPubSubV1Api
{
    Task<PubSubPublishResponse> PublishAsync(
        string projectId,
        string topic,
        IReadOnlyList<PubSubMessage> messages,
        CancellationToken cancellationToken = default
    );
}