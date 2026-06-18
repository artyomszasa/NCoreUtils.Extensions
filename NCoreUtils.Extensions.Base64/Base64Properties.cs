using System;

namespace NCoreUtils;

[Flags]
internal enum Base64Properties : uint
{
    None = 0u,
    ContainsPlus = 0b0001_00u,
    ContainsSlash = 0b0010_00u,
    ContainsHypen = 0b0100_00u,
    ContainsUnderscope = 0b1000_00u,
    ContainsPadding = 0b0001_0000_00u,
    ContainsInvalid = 0b0010_0000_00u,
    Aligned = 0b0100_0000_00u
}
