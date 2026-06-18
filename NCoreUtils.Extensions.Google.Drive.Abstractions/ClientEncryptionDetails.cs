using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

/// <summary>
/// Details about the client-side encryption applied to the file.
/// </summary>
public class ClientEncryptionDetails(
    string? encryptionState = default,
    DecryptionMetadata? decryptionMetadata = default)
{
    /// <summary>
    /// The encryption state of the file. The values expected here are: encrypted, unencrypted.
    /// </summary>
    [JsonPropertyName("encryptionState")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? EncryptionState { get; } = encryptionState;

    /// <summary>
    /// The metadata used for client-side operations.
    /// </summary>
    [JsonPropertyName("decryptionMetadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DecryptionMetadata? DecryptionMetadata { get; } = decryptionMetadata;
}
