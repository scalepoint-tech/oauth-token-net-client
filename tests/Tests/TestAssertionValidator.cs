using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using Scalepoint.OAuth.TokenClient.Internals;

namespace Tests
{
    public static class TestAssertionValidator
    {
        public static void Validate(string assertion, string audience, string clientId, X509Certificate2 signingCertificate)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = clientId,
                ValidateIssuer = true,

                ValidAudience = audience,
                ValidateAudience = true,

                IssuerSigningKey = new X509SecurityKey(signingCertificate),
                ValidateIssuerSigningKey = true,

                RequireSignedTokens = true,
                RequireExpirationTime = true
            };

            new JwtSecurityTokenHandler().ValidateToken(assertion, validationParameters, out var token);

            var jwt = (JwtSecurityToken)token;

            if (jwt.Header.Alg != SecurityAlgorithms.RsaSha256)
            {
                throw new ArgumentException($"Invalid algorithm: {jwt.Header.Alg}");
            }

            if (jwt.Subject != clientId)
            {
                throw new ArgumentException($"Invalid subject: {jwt.Subject}");
            }

            if (!string.Equals(jwt.Header.X5t, new X509SecurityKey(signingCertificate).X5t, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Invalid 'x5t' header claim: {jwt.Header.X5t}");
            }

            if (jwt.Claims.Count(c => c.Type == JwtClaimTypes.JwtId) != 1)
            {
                throw new ArgumentException($"Missing 'jti' claim");
            }
        }
    }
}
