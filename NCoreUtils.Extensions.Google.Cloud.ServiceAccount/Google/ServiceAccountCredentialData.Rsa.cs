using System.Security.Cryptography;

namespace NCoreUtils.Google;

public sealed partial class ServiceAccountCredentialData
{
#if NET6_0_OR_GREATER

    private static RSA ReadPrivateKey(string raw)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(raw);
        return rsa;
    }

#elif NETFRAMEWORK

    private static RSA ReadPrivateKey(string raw)
    {
        using var pr = new Org.BouncyCastle.OpenSsl.PemReader(new StringReader(raw));
        var keyPair = (Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair)pr.ReadObject();
        RSAParameters rsaParams = Org.BouncyCastle.Security.DotNetUtilities.ToRSAParameters((Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)keyPair.Private);
        var rsa = RSA.Create();
        rsa.ImportParameters(rsaParams);
        return rsa;
    }

#else

    private static readonly System.Text.RegularExpressions.Regex _eolRegex = new(
        "\r*\n\r*",
        System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.CultureInvariant
    );

    private static RSA ReadPrivateKey(string raw)
    {
        var key = _eolRegex.Split(raw).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        if (key.Length < 3)
        {
            throw new InvalidOperationException("Invalid private key.");
        }
        if (key[0] != "-----BEGIN PRIVATE KEY-----")
        {
            throw new InvalidOperationException("Invalid private key.");
        }
        if (key[^1] != "-----END PRIVATE KEY-----")
        {
            throw new InvalidOperationException("Invalid private key.");
        }

        var keyBody = key[1..^1];
        var base64 = string.Join(string.Empty, keyBody);
        var rsaKey = RSA.Create();
        rsaKey.ImportPkcs8PrivateKey(Convert.FromBase64String(base64), out _);
        return rsaKey;
    }

#endif
}