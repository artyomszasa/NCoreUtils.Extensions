namespace NCoreUtils.Google;

public interface IGoogleCertificateManagerApiV1
{
    Task<Operation<DnsAuthorization>> CreateDnsAuthorizationAsync(
        string projectId,
        string location,
        string dnsAuthorization,
        string domain,
        IReadOnlyDictionary<string, string>? labels = default,
        CancellationToken cancellationToken = default
    );

    Task<DnsAuthorization> GetDnsAuthorizationAsync(
        string projectId,
        string location,
        string dnsAuthorization,
        CancellationToken cancellationToken = default
    );

    Task<Operation> DeleteDnsAuthorizationAsync(
        string projectId,
        string location,
        string dnsAuthorization,
        CancellationToken cancellationToken = default
    );

    Task<Operation<Certificate>> CreateCertificateAsync(
        string projectId,
        string location,
        string certificateId,
        string name,
        ManagedCertificate managed,
        string? description = default,
        IReadOnlyDictionary<string, string>? labels = default,
        CancellationToken cancellationToken = default
    );

    Task<Certificate> GetCertificateAsync(
        string projectId,
        string location,
        string certificateId,
        CancellationToken cancellationToken = default
    );

    Task<Operation> DeleteCertificateAsync(
        string projectId,
        string location,
        string certificateId,
        CancellationToken cancellationToken = default
    );

    Task<Operation<CertificateMapEntry>> CreateCertificateMapEntryAsync(
        string projectId,
        string location,
        string mapName,
        string entryName,
        string certificateId,
        string hostname,
        string? description = default,
        IReadOnlyDictionary<string, string>? labels = default,
        CancellationToken cancellationToken = default
    );

    Task<CertificateMapEntry> GetCertificateMapEntryAsync(
        string projectId,
        string location,
        string mapName,
        string entryName,
        CancellationToken cancellationToken = default
    );

    Task<Operation> DeleteCertificateMapEntryAsync(
        string projectId,
        string location,
        string mapName,
        string entryName,
        CancellationToken cancellationToken = default
    );

    Task<Operation<DnsAuthorization>> GetCreateDnsAuthorizationOperationAsync(
        string projectId,
        string location,
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation> GetDeleteDnsAuthorizationOperationAsync(
        string projectId,
        string location,
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation<Certificate>> GetCreateCertificateOperationAsync(
        string projectId,
        string location,
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation> GetDeleteCertificateOperationAsync(
        string projectId,
        string location,
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation<CertificateMapEntry>> GetCreateCertificateMapEntryOperationAsync(
        string projectId,
        string location,
        string operationId,
        CancellationToken cancellationToken = default
    );

    Task<Operation> GetDeleteCertificateMapEntryOperationAsync(
        string projectId,
        string location,
        string operationId,
        CancellationToken cancellationToken = default
    );
}