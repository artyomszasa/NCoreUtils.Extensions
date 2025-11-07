using NCoreUtils.Proto;
using NCoreUtils.Google;
using NCoreUtils.Google.Proto;
using System.Net.Http;
using NCoreUtils.Proto.Internal;
using System.Text.Json.Serialization;
using HttpMethod = System.Net.Http.HttpMethod;

namespace NCoreUtils;

[JsonSerializable(typeof(JsonRootGoogleStorageAdminApiV1Info))]
[JsonSerializable(typeof(CreateBucketRequest))]
[JsonSerializable(typeof(GoogleBucketPatch))]
internal partial class GoogleStorageAdminApiV1SerializerContext : JsonSerializerContext { }

[ProtoClient(typeof(GoogleStorageAdminApiV1Info), typeof(GoogleStorageAdminApiV1SerializerContext))]
public partial class GoogleStorageAdminApiV1Client
{
    public const string HttpClientConfigurationName = nameof(GoogleStorageAdminApiV1Client);

    private HttpRequestMessage CreateGetBucketRequest(string bucket, string projection)
    {
        var requestUri = $"{GetCachedMethodPath(Methods.DeleteBucket)}/{Uri.EscapeDataString(bucket)}?projection={Uri.EscapeDataString(projection)}";
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateInsertBucketRequest(string projectId, CreateBucketRequest request)
    {
        var requestUri = $"{GetCachedMethodPath(Methods.InsertBucket)}?project={Uri.EscapeDataString(projectId)}";
        var req = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = ProtoJsonContent.Create(request, GoogleStorageAdminApiV1SerializerContext.Default.CreateBucketRequest, null)
        };
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private static readonly HttpMethod HttpMethodPatch
#if NETFRAMEWORK
        = new HttpMethod("PATCH");
#else
        = HttpMethod.Patch;
#endif

    private HttpRequestMessage CreatePatchBucketRequest(string bucket, GoogleBucketPatch patch, string projection)
    {
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrEmpty(bucket);
#else
        if (string.IsNullOrEmpty(bucket))
        {
            throw new ArgumentException("Bucket name must be specified.", nameof(bucket));
        }
#endif
        var requestUri = $"{GetCachedMethodPath(Methods.PatchBucket)}/{Uri.EscapeDataString(bucket)}?projection={Uri.EscapeDataString(projection)}";
        var req = new HttpRequestMessage(HttpMethodPatch, requestUri)
        {
            Content = ProtoJsonContent.Create(patch, GoogleStorageAdminApiV1SerializerContext.Default.GoogleBucketPatch, null)
        };
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateDeleteBucketRequest(string bucket)
    {
        var requestUri = $"{GetCachedMethodPath(Methods.DeleteBucket)}/{Uri.EscapeDataString(bucket)}";
        var req = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    protected override ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
        => response.HandleGoogleCloudErrorResponseAsync(cancellationToken);
}