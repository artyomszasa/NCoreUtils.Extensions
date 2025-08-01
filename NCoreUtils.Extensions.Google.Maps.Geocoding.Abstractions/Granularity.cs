namespace NCoreUtils.Google.Maps.Geocoding;

public static class Granularity
{
    /// <summary>
    /// <c>GRANULARITY_UNSPECIFIED</c> -- Do not use.
    /// </summary>
    public const string GranularityUnspecified = "GRANULARITY_UNSPECIFIED";

    /// <summary>
    /// <c>ROOFTOP</c> -- The non-interpolated location of an actual plot of land corresponding to the matched address.
    /// </summary>
    public const string Rooftop = "ROOFTOP";

    /// <summary>
    /// <c>RANGE_INTERPOLATED</c> -- Interpolated from a range of street numbers. For example, if we know that a
    /// segment of Amphitheatre Pkwy contains numbers 1600 - 1699, then 1650 might be placed halfway between it
    /// endpoints.
    /// </summary>
    public const string RangeInterpolated = "RANGE_INTERPOLATED";

    /// <summary>
    /// <c>GEOMETRIC_CENTER</c> -- The geometric center of a feature for which we have polygonal data.
    /// </summary>
    public const string GeometricCenter = "GEOMETRIC_CENTER";

    /// <summary>
    /// <c>APPROXIMATE</c> -- The geometric center of a feature for which we have polygonal data.
    /// </summary>
    public const string Approximate = "APPROXIMATE";

}

