using System.Buffers;
using System.Text.Json.Serialization;
using NCoreUtils.Google.Wallet.Proto;
using NCoreUtils.Proto;
using HttpMethod = System.Net.Http.HttpMethod;

namespace NCoreUtils.Google.Wallet;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootWalletV1ApiInfo))]
internal partial class WalletV1ApiSerializerContext : JsonSerializerContext { }

[ProtoClient(typeof(WalletV1ApiInfo), typeof(WalletV1ApiSerializerContext))]
public partial class WalletV1ApiClient
{
    public const string HttpClientConfigurationName = nameof(WalletV1ApiClient);

    protected virtual HttpRequestMessage CreateListGenericClassesRequest(string? issuerId, string? token, int? maxResults)
    {
        string path;
        var buffer = ArrayPool<char>.Shared.Rent(4 * 1024);
        try
        {
            var builder = new SpanBuilder(buffer);
            var prefix = GetCachedMethodPath(Methods.ListGenericClasses);
            builder.Append(prefix);
            var fst = !prefix.Contains('?');
            AddStringQueryParameter(ref builder, nameof(issuerId), issuerId, ref fst);
            AddStringQueryParameter(ref builder, nameof(token), token, ref fst);
            AddInt32QueryParameter(ref builder, nameof(maxResults), maxResults, ref fst);
            path = builder.ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/wallet_object.issuer");
        return request;

        static string Escape(string? value) => Uri.EscapeDataString(value ?? string.Empty);

        static void AddStringQueryParameter(ref SpanBuilder builder, string key, string? value, ref bool fst)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (fst)
                {
                    fst = false;
                    builder.Append('?');
                }
                else
                {
                    builder.Append('&');
                }
                builder.Append(key);
                builder.Append('=');
                builder.Append(Escape(value));
            }
        }

        static void AddInt32QueryParameter(ref SpanBuilder builder, string key, int? value, ref bool fst)
        {
            if (value is int v)
            {
                if (fst)
                {
                    fst = false;
                    builder.Append('?');
                }
                else
                {
                    builder.Append('&');
                }
                builder.Append(key);
                builder.Append('=');
                builder.Append(v);
            }
        }
    }

    private HttpRequestMessage CreateLookupGenericClassRequest(string id)
    {
        var uri = $"{GetCachedMethodPath(Methods.LookupGenericClass)}/{id}";
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/wallet_object.issuer");
        return request;
    }

    private ValueTask HandleLookupGenericClassErrors(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return default;
        }
        return HandleErrors(response, cancellationToken);
    }

    protected virtual Task<GenericClass?> ReadLookupGenericClassResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Task.FromResult<GenericClass?>(default);
        }
        return System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsync(response.Content, WalletV1ApiSerializerContext.Default.GenericClass, cancellationToken);
    }

    protected virtual HttpContent CreateInsertGenericClassRequestContent(GenericClass data)
    {
        return NCoreUtils.Proto.Internal.ProtoJsonContent.Create(data, WalletV1ApiSerializerContext.Default.GenericClass, JsonMediaType);
    }



    protected virtual HttpRequestMessage CreateInsertGenericClassRequest(GenericClass data)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, GetCachedMethodPath(Methods.InsertGenericClass))
        {
            Content = CreateInsertGenericClassRequestContent(data)
        };
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/wallet_object.issuer");
        return request;
    }

    protected virtual HttpContent CreateInsertGenericObjectRequestContent(GenericObject data)
    {
        return NCoreUtils.Proto.Internal.ProtoJsonContent.Create(data, WalletV1ApiSerializerContext.Default.GenericObject, JsonMediaType);
    }

    protected virtual HttpRequestMessage CreateInsertGenericObjectRequest(GenericObject data)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, GetCachedMethodPath(Methods.InsertGenericObject))
        {
            Content = CreateInsertGenericObjectRequestContent(data)
        };
        request.SetRequiredGcpScope("https://www.googleapis.com/auth/wallet_object.issuer");
        return request;
    }

    protected override ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
        => response.HandleGoogleCloudErrorResponseAsync(cancellationToken);
}