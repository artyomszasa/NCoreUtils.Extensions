using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace NCoreUtils.Google;

public static class JwtHelper
{
    public static string CreateJwtToken(ServiceAccountCredentialData cred, ScopeCollection scope)
        => CreateJwtToken(cred, default, default, scope);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Ambiguous..")]
    public static string CreateJwtToken(ServiceAccountCredentialData cred, string? email, string? algorithm, ScopeCollection scope)
    {
        var now = DateTime.Now;
        var subject = string.IsNullOrEmpty(email)
            ? new ClaimsIdentity(new Claim[]
            {
                new("scope", scope.Join(" "))
            })
            : new ClaimsIdentity(new Claim[]
            {
                new("scope", scope.Join(" ")),
                new("sub", email)
            });
        var descriptor = new SecurityTokenDescriptor()
        {
            Audience = "https://oauth2.googleapis.com/token",
            Subject = subject,
            Issuer = cred.ClientEmail,
            IssuedAt = now,
            Expires = now.AddHours(.5),
            SigningCredentials = new SigningCredentials(
                new RsaSecurityKey(cred.PrivateKeyParameters) { KeyId = cred.PrivateKeyId },
                algorithm ?? SecurityAlgorithms.RsaSha256
            )
        };
        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(descriptor);
    }
}