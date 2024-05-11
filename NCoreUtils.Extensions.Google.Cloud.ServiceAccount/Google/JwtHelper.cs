using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace NCoreUtils.Google;

public static class JwtHelper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0300:Simplify collection initialization", Justification = "Ambiguous..")]
    public static string CreateJwtToken(ServiceAccountCredentialData cred, ScopeCollection scope)
    {
        var now = DateTime.Now;
        var descriptor = new SecurityTokenDescriptor()
        {
            Audience = "https://oauth2.googleapis.com/token",
            Subject = new ClaimsIdentity(new Claim[]
            {
                new("scope", scope.Join(" "))
            }),
            Issuer = cred.ClientEmail,
            IssuedAt = now,
            Expires = now.AddHours(.5),
            SigningCredentials = new SigningCredentials(
                new RsaSecurityKey(cred.PrivateKeyParameters) { KeyId = cred.PrivateKeyId },
                SecurityAlgorithms.RsaSha256Signature
            )
        };
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }
}