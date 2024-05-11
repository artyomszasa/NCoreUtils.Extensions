namespace NCoreUtils.Google;

public static partial class HttpRequestMessageExtensions
{
    private static HttpRequestOptionsKey<ScopeCollection> HttpKeyRequiredScope { get; }
        = new HttpRequestOptionsKey<ScopeCollection>(KeyRequiredScope);

    public static void SetRequiredGcpScope(this HttpRequestMessage request, ScopeCollection scopes)
        => request.Options.Set(HttpKeyRequiredScope, scopes);

    public static bool TryGetRequiredGcpScope(this HttpRequestMessage request, out ScopeCollection scopes)
        => request.Options.TryGetValue(HttpKeyRequiredScope, out scopes);
}