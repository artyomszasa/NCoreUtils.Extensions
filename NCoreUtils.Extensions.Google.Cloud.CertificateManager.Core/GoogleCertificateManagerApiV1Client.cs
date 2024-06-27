using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using NCoreUtils.Google.Proto;
using NCoreUtils.Proto;
using NCoreUtils.Proto.Internal;

namespace NCoreUtils.Google;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(JsonRootGoogleCertificateManagerApiV1Info))]
internal partial class GoogleCertificateManagerApiV1SerializerContext : JsonSerializerContext { }

[ProtoClient(typeof(GoogleCertificateManagerApiV1Info), typeof(GoogleCertificateManagerApiV1SerializerContext))]
public partial class GoogleCertificateManagerApiV1Client
{
    private readonly struct StringEsc(bool shouldEscape, string? source, bool noSep)
    {
        public static implicit operator StringEsc(string? source)
            => new(false, source, false);

        public bool ShouldEscape { get; } = shouldEscape;

        public string? Source { get; } = source;

        public bool NoSep { get; } = noSep;

        public void Deconstruct(out bool shouldEscape, out string? source)
        {
            shouldEscape = ShouldEscape;
            source = Source;
        }
    }

    public const string HttpClientConfigurationName = nameof(GoogleCertificateManagerApiV1Client);

    private static GoogleCertificateManagerApiV1SerializerContext SerializerContext { get; } = GoogleCertificateManagerApiV1SerializerContext.Default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringEsc Esc(string? source, bool noSep = false)
        => new(true, source, noSep);

    // TODO: C# 13 --> use ReadOnlySpan params
#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    private static string UriJoin(ReadOnlySpan<StringEsc> segments)
    {
        var buffer = ArrayPool<char>.Shared.Rent(4096);
        try
        {
            var builder = new SpanBuilder(buffer);
            var enumerator = segments.GetEnumerator();
            if (enumerator.MoveNext())
            {
                AppendSegment(ref builder, enumerator.Current);
            }
            while (enumerator.MoveNext())
            {
                var segment = enumerator.Current;
                if (!segment.NoSep)
                {
                    builder.Append('/');
                }
                AppendSegment(ref builder, in segment);
            }
            return builder.ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AppendSegment(ref SpanBuilder builder, in StringEsc segment)
        {
            var (shouldEscape, value) = segment;
            if (shouldEscape)
            {
#if NET6_0_OR_GREATER
                builder.AppendUriEscaped(value ?? string.Empty);
#else
                builder.Append(Uri.EscapeDataString(value ?? string.Empty));
#endif
            }
            else
            {
                builder.Append(value ?? string.Empty);
            }
        }
    }

    private HttpRequestMessage CreateCreateDnsAuthorizationRequest(
        string projectId,
        string location,
        string dnsAuthorization,
        string domain,
        IReadOnlyDictionary<string, string>? labels = default)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.CreateDnsAuthorization),
            Esc(projectId),
            "locations",
            Esc(location),
            "dnsAuthorizations"
        ]);
        var req = new HttpRequestMessage(HttpMethod.Post, $"{requestUri}?dnsAuthorizationId={Uri.EscapeDataString(dnsAuthorization)}")
        {
            Content = ProtoJsonContent.Create(
                new DnsAuthorization(
                    name: dnsAuthorization,
                    domain: domain,
                    labels: labels
                ),
                SerializerContext.DnsAuthorization,
                default
            )
        };
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateGetDnsAuthorizationRequest(
        string projectId,
        string location,
        string dnsAuthorization)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.GetDnsAuthorization),
            Esc(projectId),
            "locations",
            Esc(location),
            "dnsAuthorizations",
            Esc(dnsAuthorization)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateDeleteDnsAuthorizationRequest(
        string projectId,
        string location,
        string dnsAuthorization)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.DeleteDnsAuthorization),
            Esc(projectId),
            "locations",
            Esc(location),
            "dnsAuthorizations",
            Esc(dnsAuthorization)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateCreateCertificateRequest(
        string projectId,
        string location,
        string certificateId,
        string name,
        ManagedCertificate managed,
        string? description = default,
        IReadOnlyDictionary<string, string>? labels = default)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.CreateCertificate),
            Esc(projectId),
            "locations",
            Esc(location),
            "certificates?certificateId=",
            Esc(certificateId, noSep: true)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = ProtoJsonContent.Create(
                new Certificate(
                    name: default!,
                    description: description,
                    labels: labels,
                    managed: managed
                ),
                SerializerContext.Certificate,
                default
            )
        };
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateGetCertificateRequest(
        string projectId,
        string location,
        string certificateId)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.GetCertificate),
            Esc(projectId),
            "locations",
            Esc(location),
            "certificates",
            Esc(certificateId)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateDeleteCertificateRequest(
        string projectId,
        string location,
        string certificateId)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.DeleteCertificate),
            Esc(projectId),
            "locations",
            Esc(location),
            "certificates",
            Esc(certificateId)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateCreateCertificateMapEntryRequest(
        string projectId,
        string location,
        string mapName,
        string entryName,
        string certificateId,
        string hostname,
        string? description = default,
        IReadOnlyDictionary<string, string>? labels = default)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.CreateCertificateMapEntry),
            Esc(projectId),
            "locations",
            Esc(location),
            "certificateMaps",
            Esc(mapName),
            "certificateMapEntries?certificateMapEntryId=",
            Esc(entryName, noSep: true)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = ProtoJsonContent.Create(
                new CertificateMapEntry(
                    name: default!,
                    description: description,
                    labels: labels,
                    certificates: [certificateId],
                    hostname: hostname
                ),
                SerializerContext.CertificateMapEntry,
                default
            )
        };
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateGetCertificateMapEntryRequest(
        string projectId,
        string location,
        string mapName,
        string entryName)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.GetCertificateMapEntry),
            Esc(projectId),
            "locations",
            Esc(location),
            "certificateMaps",
            Esc(mapName),
            "certificateMapEntries",
            Esc(entryName)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateDeleteCertificateMapEntryRequest(
        string projectId,
        string location,
        string mapName,
        string entryName)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.DeleteCertificateMapEntry),
            Esc(projectId),
            "locations",
            Esc(location),
            "certificateMaps",
            Esc(mapName),
            "certificateMapEntries",
            Esc(entryName)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateGetCreateDnsAuthorizationOperationRequest(
        string projectId,
        string location,
        string operationId)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.GetCreateDnsAuthorizationOperation),
            Esc(projectId),
            "locations",
            Esc(location),
            "operations",
            Esc(operationId)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateGetDeleteDnsAuthorizationOperationRequest(
        string projectId,
        string location,
        string operationId)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.GetDeleteDnsAuthorizationOperation),
            Esc(projectId),
            "locations",
            Esc(location),
            "operations",
            Esc(operationId)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateGetCreateCertificateOperationRequest(
        string projectId,
        string location,
        string operationId)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.GetCreateCertificateOperation),
            Esc(projectId),
            "locations",
            Esc(location),
            "operations",
            Esc(operationId)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateGetDeleteCertificateOperationRequest(
        string projectId,
        string location,
        string operationId)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.GetDeleteCertificateOperation),
            Esc(projectId),
            "locations",
            Esc(location),
            "operations",
            Esc(operationId)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateGetCreateCertificateMapEntryOperationRequest(
        string projectId,
        string location,
        string operationId)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.GetCreateCertificateMapEntryOperation),
            Esc(projectId),
            "locations",
            Esc(location),
            "operations",
            Esc(operationId)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    private HttpRequestMessage CreateGetDeleteCertificateMapEntryOperationRequest(
        string projectId,
        string location,
        string operationId)
    {
        var requestUri = UriJoin([
            GetCachedMethodPath(Methods.GetDeleteCertificateMapEntryOperation),
            Esc(projectId),
            "locations",
            Esc(location),
            "operations",
            Esc(operationId)
        ]);
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        req.SetRequiredGcpScope("https://www.googleapis.com/auth/cloud-platform");
        return req;
    }

    protected override ValueTask HandleErrors(HttpResponseMessage response, CancellationToken cancellationToken)
        => response.HandleGoogleCloudErrorResponseAsync(cancellationToken);
}