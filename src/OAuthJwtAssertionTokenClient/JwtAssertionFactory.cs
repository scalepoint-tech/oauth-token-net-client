using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace OAuthJwtAssertionTokenClient
{
    internal class JwtAssertionFactory
    {
        private readonly string _tokenEndpointUrl;
        private readonly string _clientId;
        private readonly X509Certificate2 _certificate;

        public JwtAssertionFactory(string tokenEndpointUrl, string clientId, X509Certificate2 certificate)
        {
            _tokenEndpointUrl = tokenEndpointUrl;
            _clientId = clientId;
            _certificate = certificate;
        }

        public string CreateAssertionToken()
        {
            var now = DateTimeOffset.Now;

            var jwt = new JwtSecurityToken(_clientId,
                                           _tokenEndpointUrl,
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
