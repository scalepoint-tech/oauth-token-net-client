using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace OAuthJwtAssertionTokenClient
{
    internal class JwtAssertionFactory
    {
        private readonly string _audience;
        private readonly string _clientId;
        private readonly X509Certificate2 _certificate;

        public JwtAssertionFactory(TokenClientOptions options)
        {
            _audience = options.Audience ?? options.TokenEndpointUrl;
            _clientId = options.ClientId;
            _certificate = options.Certificate;
        }

        public string CreateAssertionToken()
        {
            var now = DateTimeOffset.Now;

            var jwt = new JwtSecurityToken(_clientId,
                                           _audience,
                                           new List<Claim>()
                                           {
                                               new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
                                               new Claim(JwtClaimTypes.Subject, _clientId),
                                               new Claim(JwtClaimTypes.IssuedAt, UnixTime(now).ToString(), ClaimValueTypes.Integer64)
                                           },
                                           now.DateTime,
                                           now.DateTime.AddMinutes(1),
                                           new X509SigningCredentials(_certificate,
                                               SecurityAlgorithms.RsaSha256Signature,
                                               SecurityAlgorithms.Sha256Digest
                                            )
                        );
            var rawCertificate = Convert.ToBase64String(_certificate.Export(X509ContentType.Cert));
            jwt.Header.Add(JwtHeaderParameterNames.X5c, new[] { rawCertificate });
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(jwt);
        }

        private int UnixTime(DateTimeOffset dateTime)
        {
            return (int)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
