using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace NCoreUtils.Google;

public static class JwtHelper
{
    public static string CreateJwtToken(ServiceAccountCredentialData cred, IReadOnlyList<string> scope)
    {
        // using var rsa = RSA.Create();
        // rsa.ImportParameters(cred.PrivateKeyParameters);
        var now = DateTime.Now;
        var descriptor = new SecurityTokenDescriptor()
        {
            Audience = "https://oauth2.googleapis.com/token",
            Subject = new ClaimsIdentity(new Claim[]
            {
                new("scope", string.Join(' ', scope))
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