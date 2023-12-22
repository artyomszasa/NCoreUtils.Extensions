using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NCoreUtils.ObservableProperties;

internal static class CompilationExtensions
{
    public static bool HasLanguageVersionAtLeastEqualTo(this Compilation compilation, LanguageVersion languageVersion)
    {
        return ((CSharpCompilation)compilation).LanguageVersion >= languageVersion;
    }
}