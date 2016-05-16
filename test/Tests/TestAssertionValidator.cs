using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Scalepoint.OAuth.TokenClient;

namespace Tests
{
    public static class TestAssertionValidator
    {
        public static bool Validate(string assertion, string audience, string clientId, X509Certificate2 signingCertificate)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = clientId,
                ValidateIssuer = true,

                ValidAudience = audience,
                ValidateAudience = true,

                IssuerSigningKey = new X509AsymmetricSecurityKey(signingCertificate),
                ValidateIssuerSigningKey = true,

                RequireSignedTokens = true,
                RequireExpirationTime = true
            };

            SecurityToken token;
            try
            {
                new JwtSecurityTokenHandler().ValidateToken(assertion, validationParameters, out token);
            }
            catch (Exception)
            {
                return false;
            }

            var jwt = (JwtSecurityToken)token;

            if (jwt.Header.Alg != JwtAlgorithms.RSA_SHA256) return false;
            if (jwt.Subject != clientId) return false;
            if (jwt.Claims.Count(c => c.Type == JwtClaimTypes.JwtId) != 1) return false;

            return true;
        }
    }
}