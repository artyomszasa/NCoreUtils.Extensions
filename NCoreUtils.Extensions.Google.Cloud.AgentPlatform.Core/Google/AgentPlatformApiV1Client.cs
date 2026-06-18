using System.Text.Json;
using System.Text.Json.Serialization;
using NCoreUtils.Google.Cloud.AgentPlatform.Proto;
using NCoreUtils.Proto;
using NCoreUtils.Proto.Internal;
using HttpMethod = System.Net.Http.HttpMethod;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootAgentPlatformApiV1Info))]
[JsonSerializable(typeof(GenerateContentRequest))]
[JsonSerializable(typeof(PredictRequest))]
internal partial class AgentPlatformApiV1SerializerContext : JsonSerializerContext { }

[ProtoClient(typeof(AgentPlatformApiV1Info), typeof(AgentPlatformApiV1SerializerContext))]
public partial class AgentPlatformApiV1Client
{
    public const string HttpClientConfigurationName = nameof(AgentPlatformApiV1Client);

    private HttpRequestMessage CreateGenerateContentRequest(
        string project,
        string location,
        string publisher,
        string model,
        GenerateContentRequest req)
    {
        var pathBase = GetCachedMethodPath(Methods.Predict);
        var endpoint = $"projects/{project}/locations/{location}/publishers/{publisher}/models/{model}";
        var path = $"{pathBase}/{endpoint}:generateContent";
        // var raw = JsonSerializer.Serialize(payload, VertexAiApiV1SerializerContext.Default.PredictRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = ProtoJsonContent.Create(req, AgentPlatformApiV1SerializerContext.Default.GenerateContentRequest)
        };
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return request;
    }

    private HttpRequestMessage CreatePredictRequest(
        string project,
        string location,
        string publisher,
        string model,
        IReadOnlyList<string> prompts,
        PredictRequestParameters parameters)
    {
        var pathBase = GetCachedMethodPath(Methods.Predict);
        var endpoint = $"projects/{project}/locations/{location}/publishers/{publisher}/models/{model}";
        var path = $"{pathBase}/{endpoint}:predict";
        var payload = new PredictRequest(
            endpoint: endpoint,
            instances: prompts.Select(prompt => new PredictRequestInstance(prompt)).ToArray(),
            parameters: parameters
        );
        // var raw = JsonSerializer.Serialize(payload, VertexAiApiV1SerializerContext.Default.PredictRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = ProtoJsonContent.Create(payload, AgentPlatformApiV1SerializerContext.Default.PredictRequest)
        };
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return request;
    }

    private HttpRequestMessage CreateRunPredictRequest(
        string project,
        string location,
        string publisher,
        string model,
        IReadOnlyList<string> prompts,
        PredictRequestParameters parameters)
    {
        var pathBase = GetCachedMethodPath(Methods.RunPredict);
        var endpoint = $"projects/{project}/locations/{location}/publishers/{publisher}/models/{model}";
        var path = $"{pathBase}/{endpoint}:predictLongRunning";
        var payload = new PredictRequest(
            endpoint: endpoint,
            instances: prompts.Select(prompt => new PredictRequestInstance(prompt)).ToArray(),
            parameters: parameters
        );
        // var raw = JsonSerializer.Serialize(payload, VertexAiApiV1SerializerContext.Default.PredictRequest);
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = ProtoJsonContent.Create(payload, AgentPlatformApiV1SerializerContext.Default.PredictRequest)
        };
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return request;
    }

    private HttpRequestMessage CreateGetOperationRequest(string name)
    {
        var pathBase = GetCachedMethodPath(Methods.GetOperation);
        var path = $"{pathBase}/{name}";
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return request;
    }

    private async ValueTask HandleRunPredictErrors(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            var op = JsonSerializer.Deserialize(raw, AgentPlatformApiV1SerializerContext.Default.Operation);
            if (op?.Error is not Error error)
            {
                throw new InvalidOperationException($"Server responded with status code {response.StatusCode} and unrecognized body: {raw}.");
            }
            throw new VertexApiException(error);
        }
    }

    protected override ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
        => response.HandleGoogleCloudErrorResponseAsync(cancellationToken);
}