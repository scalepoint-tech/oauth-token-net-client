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
        }

        private static Tuple<string, TimeSpan> ParseResponse(string content)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(content);
            var accessToken = (string) body.access_token;

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new TokenEndpointException("Token endpoint response does not contain valid \"access_token\"");
            }

            var expiresInSeconds = 0;
            if (body.expires_in != null)
            {
                expiresInSeconds = body.expires_in;
            }

            var expiresIn = TimeSpan.FromSeconds(Convert.ToInt32(expiresInSeconds));
            return new Tuple<string, TimeSpan>(accessToken, expiresIn);
        }
    }
}