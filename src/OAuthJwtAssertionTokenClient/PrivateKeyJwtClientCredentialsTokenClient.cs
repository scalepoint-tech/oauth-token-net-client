using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ExpiringToken = System.Tuple<string, System.TimeSpan>;

namespace OAuthJwtAssertionTokenClient
{
    internal class PrivateKeyJwtClientCredentialsTokenClient
    {
        private readonly string _tokenEndpointUrl;
        private readonly string _clientId;
        private readonly JwtAssertionFactory _jwtAssertionFactory;

        public PrivateKeyJwtClientCredentialsTokenClient(string tokenEndpointUrl, string clientId, X509Certificate2 certificate)
        {
            _jwtAssertionFactory = new JwtAssertionFactory(tokenEndpointUrl, clientId, certificate);
            _tokenEndpointUrl = tokenEndpointUrl;
            _clientId = clientId;
        }

        public async Task<Tuple<string, TimeSpan>> GetToken(IEnumerable<string> scopes)
        {
            var tokenString = _jwtAssertionFactory.CreateAssertionToken();

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

                var expiresIn = TimeSpan.FromMinutes(Convert.ToInt32(body.expires_in));
                return new ExpiringToken(accessToken, expiresIn);
            }
        }
    }
}
