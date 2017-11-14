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
        private readonly bool _embedCertificate;

        public ClientAssertionJwtFactory(string tokenEndpointUri, string clientId, X509Certificate2 certificate, bool embedCertificate = false)
        {
            _audience = tokenEndpointUri;
            _clientId = clientId;
            _certificate = certificate;
            _embedCertificate = embedCertificate;
        }

        public string CreateAssertionToken()
        {
            var now = DateTime.Now.ToUniversalTime();
            
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
                new X509SecurityKey(_certificate),
                SecurityAlgorithms.RsaSha256
                )
            );

            if (_embedCertificate)
            {
                var rawCertificate = Convert.ToBase64String(_certificate.Export(X509ContentType.Cert));
                jwt.Header.Add(JwtHeaderParameterNames.X5c, new[] { rawCertificate });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(jwt);
        }
    }
}
