using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace NCoreUtils.Google;

public sealed partial class ServiceAccountCredentialData
    : IEquatable<ServiceAccountCredentialData>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator== (ServiceAccountCredentialData? a, ServiceAccountCredentialData? b)
        => a is null
            ? b is null
            : a.Equals(b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator!= (ServiceAccountCredentialData? a, ServiceAccountCredentialData? b)
        => !(a == b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Equals(RSAParameters p1, RSAParameters p2)
    {
        // Compare public components
        if (!BytesEqual(p1.Modulus, p2.Modulus)) { return false; }
        if (!BytesEqual(p1.Exponent, p2.Exponent)) { return false; }
        // Compare private components
        if (!BytesEqual(p1.D, p2.D)) { return false; }
        if (!BytesEqual(p1.P, p2.P)) { return false; }
        if (!BytesEqual(p1.Q, p2.Q)) { return false; }
        if (!BytesEqual(p1.DP, p2.DP)) { return false; }
        if (!BytesEqual(p1.DQ, p2.DQ)) { return false; }
        if (!BytesEqual(p1.InverseQ, p2.InverseQ)) { return false; }
        return true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool BytesEqual(byte[]? a, byte[]? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.AsSpan().SequenceEqual(b);
        }
    }

    public bool Equals([NotNullWhen(true)] ServiceAccountCredentialData? other)
        => ReferenceEquals(this, other)
            || (other is not null
                && ProjectId == other.ProjectId
                && PrivateKeyId == other.PrivateKeyId
                && Equals(PrivateKeyParameters, other.PrivateKeyParameters)
                && ClientEmail == other.ClientEmail
                && ClientId == other.ClientId
                && AuthUri == other.AuthUri
                && TokenUri == other.TokenUri
                && AuthProviderX509CertUrl == other.AuthProviderX509CertUrl
                && ClientX509CertUrl == other.ClientX509CertUrl);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => Equals(obj as ServiceAccountCredentialData);

    public override int GetHashCode() => HashCode.Combine(
        ProjectId,
        PrivateKeyId,
        ClientEmail,
        ClientId
    );
}