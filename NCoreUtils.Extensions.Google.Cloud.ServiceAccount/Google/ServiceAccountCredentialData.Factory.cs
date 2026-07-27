using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[JsonSerializable(typeof(RawServiceAccountCredentialData))]
internal partial class RawServiceAccountCredentialDataSerializerContext : JsonSerializerContext { }

public sealed partial class ServiceAccountCredentialData
{
    public static ServiceAccountCredentialData ValidateAndCreate(RawServiceAccountCredentialData raw)
    {
        if (string.IsNullOrEmpty(raw.Type))
        {
            throw new InvalidOperationException("Service account credential is missing type.");
        }
        if (raw.Type != "service_account")
        {
            throw new InvalidOperationException($"Specified credential is not a service account credential (type = \"{raw.Type}\", \"service_account\" expected).");
        }
        if (string.IsNullOrEmpty(raw.ProjectId))
        {
            throw new InvalidOperationException("Service account credential is missing project_id.");
        }
        if (string.IsNullOrEmpty(raw.PrivateKeyId))
        {
            throw new InvalidOperationException("Service account credential is missing private_key_id.");
        }
        if (string.IsNullOrEmpty(raw.PrivateKey))
        {
            throw new InvalidOperationException("Service account credential is missing private_key.");
        }
        RSAParameters privateKeyParameters;
        try
        {
            using var rsa = ReadPrivateKey(raw.PrivateKey);
            privateKeyParameters = rsa.ExportParameters(includePrivateParameters: true);
        }
        catch (Exception exn)
        {
            throw new InvalidOperationException("Failed to read private_key of service account credential as RSA/PEM private key.", exn);
        }
        if (string.IsNullOrEmpty(raw.ClientEmail))
        {
            throw new InvalidOperationException("Service account credential is missing client_email.");
        }
        if (string.IsNullOrEmpty(raw.ClientId))
        {
            throw new InvalidOperationException("Service account credential is missing client_id.");
        }
        if (string.IsNullOrEmpty(raw.AuthUri))
        {
            throw new InvalidOperationException("Service account credential is missing auth_uri.");
        }
        if (string.IsNullOrEmpty(raw.TokenUri))
        {
            throw new InvalidOperationException("Service account credential is missing token_uri.");
        }
        return new ServiceAccountCredentialData(
            projectId: raw.ProjectId,
            privateKeyId: raw.PrivateKeyId,
            privateKeyParameters: privateKeyParameters,
            clientEmail: raw.ClientEmail,
            clientId: raw.ClientId,
            authUri: raw.AuthUri,
            tokenUri: raw.TokenUri,
            authProviderX509CertUrl: raw.AuthProviderX509CertUrl,
            clientX509CertUrl: raw.ClientX509CertUrl
        );
    }

    public static async Task<ServiceAccountCredentialData> ReadFromStreamAsync(Stream source, CancellationToken cancellationToken = default)
    {
        var raw = await JsonSerializer
            .DeserializeAsync(source, RawServiceAccountCredentialDataSerializerContext.Default.RawServiceAccountCredentialData, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Failed to deserialize service account credential data from stream.");
        return ValidateAndCreate(raw);
    }

    public static async Task<ServiceAccountCredentialData> ReadFromPathAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
#if !NETFRAMEWORK
            await
#endif
            using var source = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0, FileOptions.SequentialScan | FileOptions.Asynchronous);
            return await ReadFromStreamAsync(source, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exn)
        {
            throw new InvalidOperationException("Failed to read service account credential from \"{path}\".", exn);
        }
    }

    public static Task<ServiceAccountCredentialData> ReadDefaultAsync(CancellationToken cancellationToken = default)
    {
        var path = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        if (string.IsNullOrEmpty(path))
        {
            throw new InvalidOperationException("GOOGLE_APPLICATION_CREDENTIALS environment variable is not specified or empty.");
        }
        return ReadFromPathAsync(path, cancellationToken);
    }
}