using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public record RawServiceAccountCredentialData(
    [property: JsonPropertyName("type")]
    string? Type,

    [property: JsonPropertyName("project_id")]
    string? ProjectId,

    [property: JsonPropertyName("private_key_id")]
    string? PrivateKeyId,

    [property: JsonPropertyName("private_key")]
    string? PrivateKey,

    [property: JsonPropertyName("client_email")]
    string? ClientEmail,

    [property: JsonPropertyName("client_id")]
    string? ClientId,

    [property: JsonPropertyName("auth_uri")]
    string? AuthUri,

    [property: JsonPropertyName("token_uri")]
    string? TokenUri,

    [property: JsonPropertyName("auth_provider_x509_cert_url")]
    string? AuthProviderX509CertUrl,

    [property: JsonPropertyName("client_x509_cert_url")]
    string? ClientX509CertUrl
);