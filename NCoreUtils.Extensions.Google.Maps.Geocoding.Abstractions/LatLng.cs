using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Maps.Geocoding;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
[method: JsonConstructor]
public readonly struct LatLng(double latitude, double longitude)
    : IEquatable<LatLng>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(LatLng left, LatLng right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(LatLng left, LatLng right) => !left.Equals(right);

    [JsonPropertyName("latitude")]
    public double Latitude { get; } = latitude;

    [JsonPropertyName("longitude")]
    public double Longitude { get; } = longitude;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(LatLng other)
        => Latitude == other.Latitude && Longitude == other.Longitude;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is LatLng other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Latitude, Longitude);

    public override string ToString() => $"({Latitude}, {Longitude})";
}

