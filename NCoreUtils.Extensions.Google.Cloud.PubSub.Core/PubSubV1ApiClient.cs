using System.Text.Json.Serialization;
using NCoreUtils.Google.Cloud.PubSub.Proto;
using NCoreUtils.Proto;
using NCoreUtils.Proto.Internal;

namespace NCoreUtils.Google.Cloud.PubSub;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootPubSubV1ApiInfo))]
[JsonSerializable(typeof(PubSubPublishRequest))]
internal partial class PubSubV1ClientSerializerContext : JsonSerializerContext { }

[ProtoClient(typeof(PubSubV1ApiInfo), typeof(PubSubV1ClientSerializerContext))]
public partial class PubSubV1ApiClient
{
    public const string HttpClientConfigurationName = nameof(PubSubV1ApiClient);

    private HttpRequestMessage CreatePublishRequest(string projectId, string topic, IReadOnlyList<PubSubMessage> messages)
    {
        var pathBase = GetCachedMethodPath(Methods.Publish);
        var path = $"{pathBase}/{projectId}/topics/{topic}:publish?alt=json";
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = ProtoJsonContent.Create(new PubSubPublishRequest(messages), PubSubV1ClientSerializerContext.Default.PubSubPublishRequest, default)
        };
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/pubsub");
        return request;
    }
}