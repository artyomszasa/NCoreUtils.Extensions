using System.Security.Cryptography;

namespace NCoreUtils.Google;

public sealed partial class ServiceAccountCredentialData(
    string projectId,
    string privateKeyId,
    RSAParameters privateKeyParameters,
    string clientEmail,
    string clientId,
    string authUri,
    string tokenUri,
    string? authProviderX509CertUrl,
    string? clientX509CertUrl)
{
    public string ProjectId { get; } = projectId;

    public string PrivateKeyId { get; } = privateKeyId;

    public RSAParameters PrivateKeyParameters { get; } = privateKeyParameters;

    public string ClientEmail { get; } = clientEmail;

    public string ClientId { get; } = clientId;

    public string AuthUri { get; } = authUri;

    public string TokenUri { get; } = tokenUri;

    public string? AuthProviderX509CertUrl { get; } = authProviderX509CertUrl;

    public string? ClientX509CertUrl { get; } = clientX509CertUrl;
}