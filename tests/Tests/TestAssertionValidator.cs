using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace Tests
{
    public static class TestAssertionValidator
    {
        public static void Validate(string assertion, string audience, string clientId, X509Certificate2 signingCertificate)
        {
            var securityKey = new X509SecurityKey(signingCertificate);

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = clientId,
                ValidateIssuer = true,

                ValidAudience = audience,
                ValidateAudience = true,

                IssuerSigningKey = securityKey,
                ValidateIssuerSigningKey = true,

                RequireSignedTokens = true,
                RequireExpirationTime = true
            };

            new JwtSecurityTokenHandler().ValidateToken(assertion, validationParameters, out var token);

            var jwt = (JwtSecurityToken)token;

            if (jwt.Header.Alg != "RS256") // SecurityAlgorithms.RsaSha256
            {
                throw new ArgumentException($"Invalid algorithm: {jwt.Header.Alg}");
            }

            if (jwt.Subject != clientId)
            {
                throw new ArgumentException($"Invalid subject: {jwt.Subject}");
            }

            var cert = TestCertificate.Get();
            // ReSharper disable InconsistentNaming
            // ReSharper disable once StringLiteralTypo
            string expectedX5t = new X509SecurityKey(cert).X5t;
            string expectedKid = cert.Thumbprint.ToUpperInvariant();

            var x5t = jwt.Header.X5t;
            var kid = jwt.Header.Kid;

            // ReSharper restore InconsistentNaming

            if (!string.Equals(x5t, expectedX5t, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Invalid 'x5t' header claim: {x5t}, expected {expectedX5t}");
            }

            if (!string.Equals(kid, expectedKid, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Invalid 'kid' header claim: {kid}, expected {expectedKid}");
            }

            if (jwt.Claims.Count(c => c.Type == JwtRegisteredClaimNames.Jti) != 1)
            {
                throw new ArgumentException("Missing 'jti' claim");
            }
        }
    }
}
