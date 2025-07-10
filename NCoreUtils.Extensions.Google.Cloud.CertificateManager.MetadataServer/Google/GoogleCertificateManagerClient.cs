
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Google;

public class GoogleCertificateManagerClient(IGoogleCertificateManagerApiV1 api, string projectId, string location)
    : IGoogleCertificateManagerClient
{
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly struct StringEsc(bool shouldEscape, string? source)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator StringEsc(string? source)
            => new(false, source);

        public bool ShouldEscape { get; } = shouldEscape;

        public string? Source { get; } = source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out bool shouldEscape, out string? source)
        {
            shouldEscape = ShouldEscape;
            source = Source;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringEsc Esc(string? source)
        => new(true, source);

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
                builder.Append('/');
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

    public IGoogleCertificateManagerApiV1 Api { get; } = api ?? throw new ArgumentNullException(nameof(api));

    public string ProjectId { get; } = projectId ?? throw new ArgumentNullException(nameof(projectId));

    public string Location { get; } = location ?? throw new ArgumentNullException(nameof(location));

    public Task<Operation<Certificate>> CreateCertificateAsync(
        string certificateId,
        string name,
        ManagedCertificate managed,
        string? description = null,
        IReadOnlyDictionary<string, string>? labels = null,
        CancellationToken cancellationToken = default)
        => Api.CreateCertificateAsync(
            ProjectId,
            Location,
            certificateId,
            name,
            managed,
            description,
            labels,
            cancellationToken
        );

    public Task<Operation<CertificateMapEntry>> CreateCertificateMapEntryAsync(
        string mapName,
        string entryName,
        string certificateId,
        string hostname,
        string? description = null,
        IReadOnlyDictionary<string, string>? labels = null,
        CancellationToken cancellationToken = default)
        => Api.CreateCertificateMapEntryAsync(
            ProjectId,
            Location,
            mapName,
            entryName,
            certificateId,
            hostname,
            description,
            labels,
            cancellationToken
        );

    public Task<Operation<DnsAuthorization>> CreateDnsAuthorizationAsync(
        string dnsAuthorization,
        string domain,
        IReadOnlyDictionary<string, string>? labels = null,
        CancellationToken cancellationToken = default)
        => Api.CreateDnsAuthorizationAsync(
            ProjectId,
            Location,
            dnsAuthorization,
            domain,
            labels,
            cancellationToken
        );

    public Task<Operation> DeleteCertificateAsync(string certificateId, CancellationToken cancellationToken = default)
        => Api.DeleteCertificateAsync(
            ProjectId,
            Location,
            certificateId,
            cancellationToken
        );

    public Task<Operation> DeleteCertificateMapEntryAsync(
        string mapName,
        string entryName,
        CancellationToken cancellationToken = default)
        => Api.DeleteCertificateMapEntryAsync(
            ProjectId,
            Location,
            mapName,
            entryName,
            cancellationToken
        );

    public Task<Operation> DeleteDnsAuthorizationAsync(
        string dnsAuthorization,
        CancellationToken cancellationToken = default)
        => Api.DeleteDnsAuthorizationAsync(
            ProjectId,
            Location,
            dnsAuthorization,
            cancellationToken
        );

    public Task<Certificate> GetCertificateAsync(string certificateId, CancellationToken cancellationToken = default)
        => Api.GetCertificateAsync(ProjectId, Location, certificateId, cancellationToken);

    public Task<CertificateMapEntry> GetCertificateMapEntryAsync(
        string mapName,
        string entryName,
        CancellationToken cancellationToken = default)
        => Api.GetCertificateMapEntryAsync(
            ProjectId,
            Location,
            mapName,
            entryName,
            cancellationToken
        );

    public Task<Operation<CertificateMapEntry>> GetCreateCertificateMapEntryOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default)
        => Api.GetCreateCertificateMapEntryOperationAsync(
            ProjectId,
            Location,
            operationId,
            cancellationToken
        );

    public Task<Operation<Certificate>> GetCreateCertificateOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default)
        => Api.GetCreateCertificateOperationAsync(ProjectId, Location, operationId, cancellationToken);

    public Task<Operation<DnsAuthorization>> GetCreateDnsAuthorizationOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default)
        => Api.GetCreateDnsAuthorizationOperationAsync(ProjectId, Location, operationId, cancellationToken);

    public Task<Operation> GetDeleteCertificateMapEntryOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default)
        => Api.GetDeleteCertificateMapEntryOperationAsync(ProjectId, Location, operationId, cancellationToken);

    public Task<Operation> GetDeleteCertificateOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default)
        => Api.GetDeleteCertificateOperationAsync(ProjectId, Location, operationId, cancellationToken);

    public Task<Operation> GetDeleteDnsAuthorizationOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default)
        => Api.GetDeleteDnsAuthorizationOperationAsync(ProjectId, Location, operationId, cancellationToken);

    public Task<DnsAuthorization> GetDnsAuthorizationAsync(
        string dnsAuthorization,
        CancellationToken cancellationToken = default)
        => Api.GetDnsAuthorizationAsync(ProjectId, Location, dnsAuthorization, cancellationToken);

    public string GetDnsAuthorizationFullName(string dnsAuthorizationId)
        => UriJoin([
            "projects",
            Esc(ProjectId),
            "locations",
            Esc(location),
            "dnsAuthorizations",
            Esc(dnsAuthorizationId)
        ]);

    public string GetCertificateFullName(string certificateId)
        => UriJoin([
            "projects",
            Esc(ProjectId),
            "locations",
            Esc(location),
            "certificates",
            Esc(certificateId)
        ]);
}