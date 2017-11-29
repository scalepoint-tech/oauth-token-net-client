using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Scalepoint.OAuth.TokenClient.Internals;

namespace Scalepoint.OAuth.TokenClient
{
    /// <summary>
    /// OAuth2 "client_assertion" client credentials with "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" assertion type
    /// <see href="https://tools.ietf.org/html/rfc7521#section-6.2">Assertion Framework for OAuth 2.0 Client Authentication and Authorization Grants</see>
    /// <see href="https://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication">OpenID Connect Core 1.0</see>
    /// </summary>
    public class JwtBearerClientAssertionCredentials : IClientCredentials
    {
        private readonly ClientAssertionJwtFactory _assertionFactory;

        /// <summary>
        /// Creates new JwtBearerClientAssertionCredentials
        /// </summary>
        /// <param name="tokenEndpointUri">OAuth2 token endpoint URI. Used as "aud" claim value</param>
        /// <param name="clientId">OAuth2 "client_id"</param>
        /// <param name="certificate">Certificate with private key. Certificate must be signed with SHA256. RSA keys must be 2048 bits long. Certificate must be associated with the client_id on the server</param>
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
#if !NETSTANDARD1_4
            try
            {
                // ReSharper disable once UnusedVariable
#pragma warning disable S1481 // Unused local variables should be removed
                var pk = certificate.PrivateKey;
#pragma warning restore S1481 // Unused local variables should be removed
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
#endif
        }
    }
}
