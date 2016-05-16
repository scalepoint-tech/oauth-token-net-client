using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Scalepoint.OAuth.TokenClient
{
    public class JwtBearerClientAssertionCredentials : IClientCredentials
    {
        private readonly ClientAssertionJwtFactory _assertionFactory;

        public JwtBearerClientAssertionCredentials(string tokenEndpointUri, string clientId, X509Certificate2 certificate)
        {
            _assertionFactory = new ClientAssertionJwtFactory(new TokenClientOptions()
            {
                Audience =  tokenEndpointUri,
                ClientId = clientId,
                Certificate = certificate
            });
            CredentialThumbprint = null;//TODO: DigestUtils.sha1Hex(tokenEndpointUri + clientId + CertificateUtil.getThumbprint(keyPair.getCertificate()));
        }

        public List<KeyValuePair<string, string>> PostParams => new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
            new KeyValuePair<string, string>("client_assertion", _assertionFactory.CreateAssertionToken())
        };

        public string CredentialThumbprint { get; }
    }
}