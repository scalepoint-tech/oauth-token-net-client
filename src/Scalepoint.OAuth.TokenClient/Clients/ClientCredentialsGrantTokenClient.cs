using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scalepoint.OAuth.TokenClient.Cache;
using Scalepoint.OAuth.TokenClient.Internals;

namespace Scalepoint.OAuth.TokenClient
{
    /// <summary>
    /// OAuth2 Token endpoint client with "client_credentials" grant support
    /// Tokens are cached in-memory by default
    /// </summary>
    public sealed class ClientCredentialsGrantTokenClient : IDisposable
    {
        private readonly CustomGrantTokenClient _customGrantTokenClient;
        private readonly ICache<string> _cache;
        private readonly string _partialCacheKey;

        /// <summary>
        /// Creates new ClientCredentialsGrantTokenClient
        /// </summary>
        /// <param name="tokenEndpointUri">OAuth2 Token endpoint URI</param>
        /// <param name="clientCredentials">OAuth2 client credentials</param>
        public ClientCredentialsGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials)
        {
            if (_cache == null)
            {
                _cache = new InMemoryCache<string>();
            }

            const string grantType = "client_credentials";
            _customGrantTokenClient = new CustomGrantTokenClient(tokenEndpointUri, clientCredentials, grantType);
            _partialCacheKey = string.Join(":", tokenEndpointUri, clientCredentials.CredentialThumbprint, grantType);
        }

        /// <summary>
        /// Creates new ClientCredentialsGrantTokenClient
        /// </summary>
        /// <param name="tokenEndpointUri">OAuth2 Token endpoint URI</param>
        /// <param name="clientCredentials">OAuth2 client credentials</param>
        /// <param name="cache">Token cache. Will automatically dispose if implements IDisposable</param>
        public ClientCredentialsGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials, ICache<string> cache)
            : this(tokenEndpointUri, clientCredentials)
        {
            _cache = cache;
        }

        /// <summary>
        /// Retrieve access token for the configured "client_id" and specified scopes. Request to the server is only performed if matching valid token is not in the cache
        /// </summary>
        /// <param name="scopes">OAuth2 scopes to request</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Access token</returns>
        /// <exception cref="TokenEndpointException">Exception during token endpoint communication</exception>
        public Task<string> GetTokenAsync(string[] scopes, CancellationToken token = default(CancellationToken))
        {
            var cacheKey = string.Join(":", _partialCacheKey, ScopeHelper.ToScopeString(scopes));
            return _cache.GetOrCreateAsync(cacheKey,
                ct => _customGrantTokenClient.GetTokenInternalAsync(new List<KeyValuePair<string, string>>(), scopes, ct),
                token);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ClientCredentialsGrantTokenClient()
        {
            Dispose(false);
        }

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _customGrantTokenClient.Dispose();
                if (_cache is IDisposable disposableCache)
                {
                    disposableCache.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
