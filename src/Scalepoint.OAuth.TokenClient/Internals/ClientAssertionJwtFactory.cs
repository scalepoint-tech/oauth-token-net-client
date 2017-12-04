using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
#if NET45
using System.IdentityModel.Tokens;
#else
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
#endif

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

#if NET45
            var signingCredentials = new X509SigningCredentials(_certificate);
#else
            var securityKey = new X509SecurityKey(_certificate);
            var signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.RsaSha256
            );
#endif

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
                signingCredentials
            );

#if NET45
            jwt.Header.Add(JwtHeaderParameterNames.Kid, _certificate.Thumbprint);
#else
// ReSharper disable once PossibleNullReferenceException
#pragma warning disable CA1308 // Normalize strings to uppercase
            jwt.Header.Add(JwtHeaderParameterNames.X5t, securityKey.X5t);
#pragma warning restore CA1308 // Normalize strings to uppercase
#endif

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(jwt);
        }
    }
}
