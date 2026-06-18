using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

/// <summary>
/// Representation of the CSE DecryptionMetadata.
/// </summary>
public class DecryptionMetadata(
    string? wrappedKey = default,
    string? kaclsId = default,
    string? kaclsName = default,
    string? aes256GcmChunkSize = default,
    string? jwt = default,
    string? keyFormat = default,
    string? encryptionResourceKeyHash = default)
{
    /// <summary>
    /// The URL-safe Base64 encoded wrapped key used to encrypt the contents of the file.
    /// </summary>
    [JsonPropertyName("wrappedKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WrappedKey { get; } = wrappedKey;

    /// <summary>
    /// The ID of the KACLS (Key ACL Service) used to encrypt the file.
    /// </summary>
    [JsonPropertyName("kaclsId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? KaclsId { get; } = kaclsId;

    /// <summary>
    /// The name of the KACLS (Key ACL Service) used to encrypt the file.
    /// </summary>
    [JsonPropertyName("kaclsName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? KaclsName { get; } = kaclsName;

    /// <summary>
    /// Chunk size used if content was encrypted with the AES 256 GCM Cipher.
    /// </summary>
    [JsonPropertyName("aes256GcmChunkSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Aes256GcmChunkSize { get; } = aes256GcmChunkSize;

    /// <summary>
    /// The signed JSON Web Token (JWT) which can be used to authorize the requesting user with the Key ACL Service (KACLS).
    /// </summary>
    [JsonPropertyName("jwt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Jwt { get; } = jwt;

    /// <summary>
    /// Key format for the unwrapped key. Must be tinkAesGcmKey.
    /// </summary>
    [JsonPropertyName("keyFormat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? KeyFormat { get; } = keyFormat;

    /// <summary>
    /// The URL-safe Base64 encoded HMAC-SHA256 digest of the resource metadata with its DEK (Data Encryption Key).
    /// </summary>
    [JsonPropertyName("encryptionResourceKeyHash")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? EncryptionResourceKeyHash { get; } = encryptionResourceKeyHash;
}
