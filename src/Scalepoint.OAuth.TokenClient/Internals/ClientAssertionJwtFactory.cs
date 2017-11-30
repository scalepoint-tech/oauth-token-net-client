using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace Scalepoint.OAuth.TokenClient.Internals
{
    public class ClientAssertionJwtFactory
    {
        private readonly string _audience;
        private readonly string _clientId;
        private readonly X509Certificate2 _certificate;

        public ClientAssertionJwtFactory(string tokenEndpointUri, string clientId, X509Certificate2 certificate)
        {
            _audience = tokenEndpointUri;
            _clientId = clientId;
            _certificate = certificate;
        }

        public string CreateAssertionToken()
        {
            var now = DateTime.Now.ToUniversalTime();

            var securityKey = new X509SecurityKey(_certificate);
            var jwt = new JwtSecurityToken(_clientId,
                _audience,
                new List<Claim>
                {
                    new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
                    new Claim(JwtClaimTypes.Subject, _clientId),
                    new Claim(JwtClaimTypes.IssuedAt, EpochTime.GetIntDate(now).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64)
                },
                now,
                now.AddMinutes(1),
                new SigningCredentials(
                securityKey,
                SecurityAlgorithms.RsaSha256
                )
            );

            // ReSharper disable once PossibleNullReferenceException
#pragma warning disable CA1308 // Normalize strings to uppercase
            jwt.Header.Add(JwtHeaderParameterNames.X5t, securityKey.X5t);
#pragma warning restore CA1308 // Normalize strings to uppercase

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(jwt);
        }
    }
}
