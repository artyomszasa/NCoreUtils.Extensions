using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NCoreUtils.ObservableProperties;

internal class ObservableFieldTarget(SemanticModel semanticModel, INamedTypeSymbol host, IFieldSymbol field)
{
    public SemanticModel SemanticModel { get; } = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));

    public INamedTypeSymbol Host { get; } = host ?? throw new ArgumentNullException(nameof(host));

    public IFieldSymbol Field { get; } = field ?? throw new ArgumentNullException(nameof(field));
}

internal class ObservableClassTarget(INamedTypeSymbol host, IReadOnlyList<IFieldSymbol> fields)
{
    public INamedTypeSymbol Host { get; } = host ?? throw new ArgumentNullException(nameof(host));

    public IReadOnlyList<IFieldSymbol> Fields { get; } = fields ?? throw new ArgumentNullException(nameof(fields));
}