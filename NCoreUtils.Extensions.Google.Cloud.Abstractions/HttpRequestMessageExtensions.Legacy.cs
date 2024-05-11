namespace NCoreUtils.Google;

public static partial class HttpRequestMessageExtensions
{
    public static void SetRequiredGcpScope(this HttpRequestMessage request, ScopeCollection scopes)
        => request.Properties[KeyRequiredScope] = scopes;

    public static bool TryGetRequiredGcpScope(this HttpRequestMessage request, out ScopeCollection scopes)
    {
        if (request.Properties.TryGetValue(KeyRequiredScope, out var boxed) && boxed is ScopeCollection value)
        {
            scopes = value;
            return true;
        }
        scopes = default;
        return false;
    }
}