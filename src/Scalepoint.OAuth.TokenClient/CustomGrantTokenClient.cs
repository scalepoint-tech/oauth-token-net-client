using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scalepoint.OAuth.TokenClient
{
    /// <summary>
    /// Abstract token endpoint client, extendable to handle custom token endpoint grants
    /// </summary>
    public abstract class CustomGrantTokenClient
    {
        private readonly TokenEndpointHttpClient _tokenEndpointHttpClient;
        private readonly IClientCredentials _clientCredentials;
        private readonly string _partialCacheKey;
        private readonly ITokenCache _cache;

        /// <summary>
        /// Create new CustomGrantTokenClient
        /// </summary>
        /// <param name="tokenEndpointUri">OAuth2 token endpoint URI</param>
        /// <param name="clientCredentials">Client credentials</param>
        /// <param name="cache">Token cache</param>
        protected CustomGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials, ITokenCache cache)
        {
            _tokenEndpointHttpClient = new TokenEndpointHttpClient(tokenEndpointUri);
            _clientCredentials = clientCredentials;
            _partialCacheKey = string.Join(":", tokenEndpointUri, clientCredentials.CredentialThumbprint);
            _cache = cache;
        }

        /// <summary>
        /// Retrieve access token for the configured "client_id" and specified scopes. Request to the server is only performed if matching valid token is not in the cache
        /// </summary>
        /// <param name="parameters">Grant-specific parameters</param>
        /// <param name="scopes">OAuth2 scopes to request</param>
        /// <returns>Access token</returns>
        protected Task<string> GetTokenInternal(IEnumerable<KeyValuePair<string, string>> parameters, params string[] scopes)
        {
            var scopeString = scopes != null && scopes.Length >= 1
                ? string.Join(" ", scopes)
                : null;

            var cacheKey = string.Join(":", _partialCacheKey, GrantType, scopeString, parameters.GetHashCode().ToString());

            return _cache.GetAsync(cacheKey, () =>
            {
                var form = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", GrantType)
                };

                form.AddRange(_clientCredentials.PostParams);

                form.AddRange(parameters);

                if (scopeString != null)
                {
                    form.Add(new KeyValuePair<string, string>("scope", scopeString));
                }

                return _tokenEndpointHttpClient.GetToken(form);
            });
        }

        /// <summary>
        /// Grant type (i.e. "client_credentials")
        /// </summary>
        protected abstract string GrantType { get; }
    }
}