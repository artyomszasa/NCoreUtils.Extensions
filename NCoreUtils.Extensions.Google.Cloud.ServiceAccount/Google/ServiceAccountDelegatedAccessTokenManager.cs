
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace NCoreUtils.Google;

public class ServiceAccountDelegatedAccessTokenManager(
    ILogger<ServiceAccountDelegatedAccessTokenManager> logger,
    ServiceAccountCredentialData credential,
    string email,
    IHttpClientFactory? httpClientFactory = default)
    : ServiceAccountAccessTokenManager(logger, credential, httpClientFactory)
{
    public string Email { get; } = email;

    protected override string CreateAssertion(ScopeCollection scope)
        => JwtHelper.CreateJwtToken(Credential, Email, SecurityAlgorithms.RsaSha256, scope);
}