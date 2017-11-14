using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Scalepoint.OAuth.TokenClient.Internals
{
    internal class TokenEndpointHttpClient
    {
        private static readonly ConcurrentDictionary<string, HttpClient> ClientsPool = new ConcurrentDictionary<string, HttpClient>();
        private readonly string _tokenEndpointUri;

        public TokenEndpointHttpClient(string tokenEndpointUri)
        {
            _tokenEndpointUri = tokenEndpointUri;
        }

        public async Task<(string token, TimeSpan expiresIn)> GetToken(List<KeyValuePair<string, string>> parameters)
        {
            var client = GetClient();
            var requestBody = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync(_tokenEndpointUri, requestBody).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new TokenEndpointException($"{(int)response.StatusCode} {response.ReasonPhrase}: {content}");
            }

            try
            {
                return ParseResponse(content);
            }
            catch (Exception e)
            {
                throw new TokenEndpointException("Invalid token response", e);
            }
        }

        private HttpClient GetClient()
        {
            return ClientsPool.GetOrAdd(_tokenEndpointUri, uri => new HttpClient());
        }

        private static (string, TimeSpan) ParseResponse(string content)
        {
            var body = JObject.Parse(content);
            var accessToken = body.Property("access_token").Value.Value<string>();
            var expiresInJson = body.Property("expires_in");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new TokenEndpointException("Token endpoint response does not contain valid \"access_token\"");
            }

            var expiresInSeconds = 0;
            if (expiresInJson != null)
            {
                expiresInSeconds = expiresInJson.Value.Value<int>();
            }

            var expiresIn = TimeSpan.FromSeconds(Convert.ToInt32(expiresInSeconds));
            return (accessToken, expiresIn);
        }
    }
}
