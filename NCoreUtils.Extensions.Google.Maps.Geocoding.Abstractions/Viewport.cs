using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Maps.Geocoding;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
[method: JsonConstructor]
public readonly struct Viewport(LatLng low, LatLng high)
    : IEquatable<Viewport>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Viewport left, Viewport right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Viewport left, Viewport right) => !left.Equals(right);

    [JsonPropertyName("low")]
    public LatLng Low { get; } = low;

    [JsonPropertyName("high")]
    public LatLng High { get; } = high;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Viewport other)
        => Low == other.Low && High == other.High;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is LatLng other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Low, High);
}

