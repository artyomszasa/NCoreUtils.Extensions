using System.Text.Json.Serialization;
using NCoreUtils.Google.Cloud.Monitoring.Proto;
using NCoreUtils.Proto;
using NCoreUtils.Proto.Internal;
using HttpMethod = System.Net.Http.HttpMethod;

namespace NCoreUtils.Google.Cloud.Monitoring;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootMonitoringV3ApiInfo))]
[JsonSerializable(typeof(CreateTimeSeriesRequest))]
[JsonSerializable(typeof(Point))]
internal partial class MonitoringV3ApiSerializerContext : JsonSerializerContext { }

[ProtoClient(typeof(MonitoringV3ApiInfo), typeof(MonitoringV3ApiSerializerContext))]
public partial class MonitoringV3ApiClient
{
    public const string HttpClientConfigurationName = nameof(MonitoringV3ApiClient);

    private HttpRequestMessage CreateCreateTimeSeriesRequest(string projectId, IReadOnlyList<TimeSeries> timeSeries)
    {
        var pathBase = GetCachedMethodPath(Methods.CreateTimeSeries);
        var path = $"{pathBase}/{projectId}/timeSeries";
        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = ProtoJsonContent.Create(new CreateTimeSeriesRequest(timeSeries), MonitoringV3ApiSerializerContext.Default.CreateTimeSeriesRequest, default)
        };
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/monitoring.write");
        return request;
    }

    protected override ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
        => response.HandleGoogleCloudErrorResponseAsync(cancellationToken);
}