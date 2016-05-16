using System.Collections.Generic;

namespace Scalepoint.OAuth.TokenClient
{
    public class ClientSecretCredentials : IClientCredentials
    {
        private readonly string _clientId;
        private readonly string _clientSecret;

        public ClientSecretCredentials(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            CredentialThumbprint = (clientId + clientSecret).Sha1Hex();
        }

        public List<KeyValuePair<string, string>> PostParams => new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret)
        };

        public string CredentialThumbprint { get; }
    }
}