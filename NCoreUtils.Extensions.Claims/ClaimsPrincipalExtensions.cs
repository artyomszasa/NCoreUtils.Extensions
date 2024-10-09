using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace NCoreUtils;

public static class ClaimsPrincipalExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetName([NotNullWhen(true)] this ClaimsPrincipal? user, [MaybeNullWhen(false)] out string name)
    {
        if (user is not null)
        {
            name = user.FindFirst(ClaimTypes.Name)?.Value;
            return !string.IsNullOrEmpty(name);
        }
        name = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetNameOrDefault(this ClaimsPrincipal? user)
        => user.TryGetName(out var name) ? name : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetName(this ClaimsPrincipal? user)
        => user.GetNameOrDefault() ?? throw new ClaimNotFoundException(ClaimTypes.Name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetEmail([NotNullWhen(true)] this ClaimsPrincipal? user, [MaybeNullWhen(false)] out string email)
    {
        if (user is not null)
        {
            email = user.FindFirst(ClaimTypes.Email)?.Value;
            return !string.IsNullOrEmpty(email);
        }
        email = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetEmailOrDefault(this ClaimsPrincipal? user)
        => user.TryGetEmail(out var email) ? email : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetEmail(this ClaimsPrincipal? user)
        => user.GetEmailOrDefault() ?? throw new ClaimNotFoundException(ClaimTypes.Email);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetSid([NotNullWhen(true)] this ClaimsPrincipal? user, [MaybeNullWhen(false)] out string sid)
    {
        if (user is not null)
        {
            sid = user.FindFirst(ClaimTypes.Sid)?.Value;
            return !string.IsNullOrEmpty(sid);
        }
        sid = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetSidOrDefault(this ClaimsPrincipal? user)
        => user.TryGetSid(out var sid) ? sid : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetSid(this ClaimsPrincipal? user)
        => user.GetSidOrDefault() ?? throw new ClaimNotFoundException(ClaimTypes.Sid);
}