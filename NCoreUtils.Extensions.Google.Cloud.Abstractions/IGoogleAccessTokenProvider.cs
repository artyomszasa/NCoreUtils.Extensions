namespace NCoreUtils.Google;

public interface IGoogleAccessTokenProvider
{
    ValueTask<string> GetAccessTokenAsync(ScopeCollection scope, CancellationToken cancellationToken = default);
}