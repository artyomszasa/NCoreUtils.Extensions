using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class RawServiceAccountCredentialData(
    string? type,
    string? projectId,
    string? privateKeyId,
    string? privateKey,
    string? clientEmail,
    string? clientId,
    string? authUri,
    string? tokenUri,
    string? authProviderX509CertUrl,
    string? clientX509CertUrl)
{
    [property: JsonPropertyName("type")]
    public string? Type { get; } = type;

    [property: JsonPropertyName("project_id")]
    public string? ProjectId { get; } = projectId;

    [property: JsonPropertyName("private_key_id")]
    public string? PrivateKeyId { get; } = privateKeyId;

    [property: JsonPropertyName("private_key")]
    public string? PrivateKey { get; } = privateKey;

    [property: JsonPropertyName("client_email")]
    public string? ClientEmail { get; } = clientEmail;

    [property: JsonPropertyName("client_id")]
    public string? ClientId { get; } = clientId;

    [property: JsonPropertyName("auth_uri")]
    public string? AuthUri { get; } = authUri;

    [property: JsonPropertyName("token_uri")]
    public string? TokenUri { get; } = tokenUri;

    [property: JsonPropertyName("auth_provider_x509_cert_url")]
    public string? AuthProviderX509CertUrl { get; } = authProviderX509CertUrl;

    [property: JsonPropertyName("client_x509_cert_url")]
    public string? ClientX509CertUrl { get; } = clientX509CertUrl;
}