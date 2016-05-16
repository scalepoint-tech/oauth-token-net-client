using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Scalepoint.OAuth.TokenClient
{
    internal class TokenEndpointHttpClient
    {
        private readonly string _tokenEndpointUri;

        public TokenEndpointHttpClient(string tokenEndpointUri)
        {
            _tokenEndpointUri = tokenEndpointUri;
        }

        public async Task<Tuple<string, TimeSpan>> GetToken(List<KeyValuePair<string, string>> parameters)
        {
            using (var client = new HttpClient())
            {
                var requestBody = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync(_tokenEndpointUri, requestBody).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new TokenEndpointException(content);
                }

                return ParseResponse(content);
            }
        }

        private static Tuple<string, TimeSpan> ParseResponse(string content)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(content);
            var accessToken = (string) body.access_token;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new TokenEndpointException("Token endpoint response does not contain valid \"access_token\"");
            }

            if (body.expires_in == null)
            {
                throw new TokenEndpointException("Token endpoint response does not contain valid \"expires_in\"");
            }

            var expiresIn = TimeSpan.FromSeconds(Convert.ToInt32(body.expires_in));
            return new Tuple<string, TimeSpan>(accessToken, expiresIn);
        }
    }
}