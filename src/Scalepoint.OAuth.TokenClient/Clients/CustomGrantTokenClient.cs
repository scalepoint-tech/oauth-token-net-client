using System.Collections.Generic;
using System.Threading.Tasks;
using Scalepoint.OAuth.TokenClient.Cache;
using Scalepoint.OAuth.TokenClient.Internals;
using NameValuePair=System.Collections.Generic.KeyValuePair<string, string>;

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
        protected Task<string> GetTokenInternalAsync(IList<NameValuePair> parameters, params string[] scopes)
        {
            var scopeString = scopes != null && scopes.Length >= 1
                ? string.Join(" ", scopes)
                : null;

            var cacheKey = string.Join(":", _partialCacheKey, GrantType, scopeString, GetParametersHashCode(parameters).ToString());

            return _cache.GetAsync(cacheKey, () =>
            {
                var form = new List<NameValuePair>
                {
                    new NameValuePair("grant_type", GrantType)
                };

                form.AddRange(_clientCredentials.PostParams);

                form.AddRange(parameters);

                if (scopeString != null)
                {
                    form.Add(new NameValuePair("scope", scopeString));
                }

                return _tokenEndpointHttpClient.GetToken(form);
            });
        }

        private static int GetParametersHashCode(IList<NameValuePair> parameters)
        {
            unchecked
            {
                int hash = 19;
                foreach (var parameter in parameters)
                {
                    hash = hash * 31 + parameter.Key.GetHashCode();
                    hash = hash * 31 + parameter.Value.GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// Grant type (i.e. "client_credentials")
        /// </summary>
        protected abstract string GrantType { get; }
    }
}