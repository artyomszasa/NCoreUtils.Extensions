namespace NCoreUtils.Google.Cloud.PubSub;

public interface IPubSubV1Api
{
    Task<PubSubPublishResponse> PublishAsync(
        string projectId,
        string topic,
        IReadOnlyList<PubSubMessage> messages,
        CancellationToken cancellationToken = default
    );

    Task<PubSubPullResponse> PullAsync(
        string projectId,
        string subscription,
        int maxMessages,
        CancellationToken cancellationToken = default
    );

    Task AcknowledgeAsync(
        string projectId,
        string subscription,
        IReadOnlyList<string> ackIds,
        CancellationToken cancellationToken = default
    );
}