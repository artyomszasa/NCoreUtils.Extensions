using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

#if NET8_0_OR_GREATER

[JsonPolymorphic(TypeDiscriminatorPropertyName = "@type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
[JsonDerivedType(typeof(GoogleRpcBadRequest), typeDiscriminator: "type.googleapis.com/google.rpc.BadRequest")]
[JsonDerivedType(typeof(GoogleRpcPreconditionFailure), typeDiscriminator: "type.googleapis.com/google.rpc.PreconditionFailure")]
public class GoogleRpcErrorDetails { }

public class GoogleRpcPreconditionViolation
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class GoogleRpcPreconditionFailure : GoogleRpcErrorDetails
{
    [JsonPropertyName("violations")]
    public IReadOnlyList<GoogleRpcPreconditionViolation> Violations { get; set; }
}

public class GoogleRpcBadRequestFieldViolation
{
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class GoogleRpcBadRequest : GoogleRpcErrorDetails
{
    [JsonPropertyName("fieldViolations")]
    public IReadOnlyList<GoogleRpcBadRequestFieldViolation>? FieldViolations { get; set; }
}

#endif

public class GoogleErrorData
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string? Status { get; set; }

#if NET8_0_OR_GREATER

    [JsonPropertyName("details")]
    public IReadOnlyList<GoogleRpcErrorDetails>? Details { get; set; }

#endif

    [JsonPropertyName("errors")]
    public IReadOnlyList<GoogleErrorDetails>? Errors { get; set; }
}