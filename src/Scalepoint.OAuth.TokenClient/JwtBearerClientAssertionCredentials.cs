using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Scalepoint.OAuth.TokenClient
{
    public class JwtBearerClientAssertionCredentials : IClientCredentials
    {
        private readonly ClientAssertionJwtFactory _assertionFactory;

        public JwtBearerClientAssertionCredentials(string tokenEndpointUri, string clientId, X509Certificate2 certificate)
        {
            ValidateCertificate(certificate);
            _assertionFactory = new ClientAssertionJwtFactory(tokenEndpointUri, clientId, certificate);
            CredentialThumbprint = (tokenEndpointUri + clientId + certificate.Thumbprint).Sha1Hex();
        }

        public List<KeyValuePair<string, string>> PostParams => new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
            new KeyValuePair<string, string>("client_assertion", _assertionFactory.CreateAssertionToken())
        };

        public string CredentialThumbprint { get; }

        private static void ValidateCertificate(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentException("Certificate is required", nameof(certificate));
            }
            if (!certificate.HasPrivateKey)
            {
                throw new ArgumentException("Certificate has no private key and cannot be used for token signing", nameof(certificate));
            }
            try
            {
                // ReSharper disable once UnusedVariable
                var pk = certificate.PrivateKey;
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    "Certificate has a private key, but it cannot be accessed. " +
                    "Either user account has no permission to access private key or this is a CNG certificate. " +
                    "Only CSP certificates are fully supported by X509Certificate2.",
                    nameof(certificate),
                    e);
            }
        }
    }
}