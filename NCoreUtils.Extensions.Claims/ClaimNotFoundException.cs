using System.Collections.Generic;

namespace NCoreUtils;

public class ClaimNotFoundException(string claimType)
    : KeyNotFoundException($"Unable to get value for claim of type \"{claimType}\".")
{
    public string ClaimType { get; } = claimType;
}