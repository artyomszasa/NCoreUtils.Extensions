using System.Collections.Immutable;

namespace NCoreUtils;

internal class ArgumentData(string name, string type)
{
    public string Name { get; } = name;

    public string Type { get; } = type;
}

internal class ConstructorData(ImmutableArray<ArgumentData> arguments)
{
    public ImmutableArray<ArgumentData> Arguments { get; } = arguments;
}

internal class FactorifyTarget(
    bool isStruct,
    string @namespace,
    string name,
    ConstructorData[] constructors)
{
    public bool IsStruct { get; } = isStruct;
    public string Name { get; } = name;
    public ConstructorData[] Constructors { get; } = constructors;
    public string Namespace { get; } = @namespace;
}
