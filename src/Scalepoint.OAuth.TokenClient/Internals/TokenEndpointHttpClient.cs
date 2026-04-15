using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scalepoint.OAuth.TokenClient.Internals
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    internal class TokenEndpointHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _tokenEndpointUri;

        public TokenEndpointHttpClient(string tokenEndpointUri)
        {
            _tokenEndpointUri = tokenEndpointUri;
        }

        public async Task<Tuple<string, TimeSpan>> GetToken(List<KeyValuePair<string, string>> parameters, CancellationToken token)
        {
            var requestBody = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync(_tokenEndpointUri, requestBody, token).ConfigureAwait(false);
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

        private static Tuple<string, TimeSpan> ParseResponse(string content)
        {
            using (var body = JsonDocument.Parse(content))
            {
                var root = body.RootElement;

                if (!root.TryGetProperty("access_token", out var accessTokenElement))
                {
                    throw new TokenEndpointException("Token endpoint response does not contain valid \"access_token\"");
                }

                var accessToken = accessTokenElement.GetString();

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    throw new TokenEndpointException("Token endpoint response does not contain valid \"access_token\"");
                }

                var expiresInSeconds = 0;
                if (root.TryGetProperty("expires_in", out var expiresInElement))
                {
                    expiresInSeconds = expiresInElement.GetInt32();
                }

                var expiresIn = TimeSpan.FromSeconds(Convert.ToInt32(expiresInSeconds));
                return new Tuple<string, TimeSpan>(accessToken, expiresIn);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TokenEndpointHttpClient()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
        }
    }
}
