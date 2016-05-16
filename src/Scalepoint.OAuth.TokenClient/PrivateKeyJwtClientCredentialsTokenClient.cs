using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ExpiringToken = System.Tuple<string, System.TimeSpan>;

namespace Scalepoint.OAuth.TokenClient
{
    internal class PrivateKeyJwtClientCredentialsTokenClient
    {
        private readonly string _tokenEndpointUrl;
        private readonly string _clientId;
        private readonly ClientAssertionJwtFactory _clientAssertionJwtFactory;

        public PrivateKeyJwtClientCredentialsTokenClient(TokenClientOptions options)
        {
            _clientAssertionJwtFactory = new ClientAssertionJwtFactory(options);
            _tokenEndpointUrl = options.TokenEndpointUrl;
            _clientId = options.ClientId;
        }

        public async Task<Tuple<string, TimeSpan>> GetToken(IEnumerable<string> scopes)
        {
            var tokenString = _clientAssertionJwtFactory.CreateAssertionToken();

            using (var client = new HttpClient())
            {
                var scopeString = string.Join(" ", scopes);
                var requestBody = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", _clientId },
                    { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                    { "client_assertion", tokenString },
                    { "grant_type", "client_credentials" },
                    { "scope", scopeString }
                });
                var response = await client.PostAsync(_tokenEndpointUrl, requestBody).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new TokenEndpointException(content);
                }

                var body = JsonConvert.DeserializeObject<dynamic>(content);
                var accessToken = (string)body.access_token;

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new TokenEndpointException("Token endpoint response does not contain valid \"access_token\"");
                }
                if (body.expires_in == null)
                {
                    throw new TokenEndpointException("Token endpoint response does not contain valid \"expires_in\"");
                }

                var expiresIn = TimeSpan.FromSeconds(Convert.ToInt32(body.expires_in));
                return new ExpiringToken(accessToken, expiresIn);
            }
        }
    }
}
