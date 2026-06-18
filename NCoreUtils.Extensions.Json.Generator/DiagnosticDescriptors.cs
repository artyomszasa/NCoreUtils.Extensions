namespace NCoreUtils.Json;

internal static class DiagnosticDescriptors
{
    public static DiagnosticDescriptor GenericError = new(
        "NCU0000",
        "Generic Error",
        "{0}: {1} {2}",
        "EnumConverterGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}