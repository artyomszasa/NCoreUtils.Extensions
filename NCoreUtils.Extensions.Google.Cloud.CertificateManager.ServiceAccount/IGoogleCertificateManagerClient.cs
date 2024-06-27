using NCoreUtils.Google;

namespace NCoreUtils;

public interface IGoogleCertificateManagerClient
{
    Task<Operation<DnsAuthorization>> CreateDnsAuthorizationAsync(
        string dnsAuthorization,
        string domain,
        IReadOnlyDictionary<string, string>? labels = default,
        CancellationToken cancellationToken = default
    );

    Task<DnsAuthorization> GetDnsAuthorizationAsync(
        string dnsAuthorization,
        CancellationToken cancellationToken = default
    );

    Task<Operation> DeleteDnsAuthorizationAsync(
        string dnsAuthorization,
        CancellationToken cancellationToken = default
    );

    Task<Operation<Certificate>> CreateCertificateAsync(
        string certificateId,
        string name,
        ManagedCertificate managed,
        string? description = default,
        IReadOnlyDictionary<string, string>? labels = default,
        CancellationToken cancellationToken = default
    );

    Task<Certificate> GetCertificateAsync(
        string certificateId,
        CancellationToken cancellationToken = default
    );

    Task<Operation> DeleteCertificateAsync(
        string certificateId,
        CancellationToken cancellationToken = default
    );

    Task<Operation<CertificateMapEntry>> CreateCertificateMapEntryAsync(
        string mapName,
        string entryName,
        string certificateId,
        string hostname,
        string? description = default,
        IReadOnlyDictionary<string, string>? labels = default,
        CancellationToken cancellationToken = default
    );

    Task<CertificateMapEntry> GetCertificateMapEntryAsync(
        string mapName,
        string entryName,
        CancellationToken cancellationToken = default
    );

    Task<Operation> DeleteCertificateMapEntryAsync(
        string mapName,
        string entryName,
        CancellationToken cancellationToken = default
    );

    Task<Operation<DnsAuthorization>> GetCreateDnsAuthorizationOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation> GetDeleteDnsAuthorizationOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation<Certificate>> GetCreateCertificateOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation> GetDeleteCertificateOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation<CertificateMapEntry>> GetCreateCertificateMapEntryOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation> GetDeleteCertificateMapEntryOperationAsync(
        string operationId,
        CancellationToken cancellationToken = default
    );

    string GetDnsAuthorizationFullName(string dnAuthorizationId);

    string GetCertificateFullName(string certificateId);
}