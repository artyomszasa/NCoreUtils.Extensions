using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NCoreUtils.ObservableProperties;

internal static class CompilationExtensions
{
    public static bool HasLanguageVersionAtLeastEqualTo(this Compilation compilation, LanguageVersion languageVersion)
    {
        return ((CSharpCompilation)compilation).LanguageVersion >= languageVersion;
    }

    public static bool TryGetFirst(this ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, string name, out TypedConstant value)
    {
        foreach (var kv in namedArguments)
        {
            if (kv.Key == name)
            {
                value = kv.Value;
                return true;
            }
        }
        value = default;
        return false;
    }
}