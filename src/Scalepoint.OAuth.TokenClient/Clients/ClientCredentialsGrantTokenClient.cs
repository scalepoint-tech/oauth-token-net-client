using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Scalepoint.OAuth.TokenClient
{
    /// <summary>
    /// OAuth2 Token endpoint client with "client_credentials" grant support
    /// Tokens are cached in-memory by default
    /// </summary>
    public class ClientCredentialsGrantTokenClient : CustomGrantTokenClient
    {

#if NETSTANDARD2_0
        private static readonly Lazy<IDistributedCache> DefaultTokenCache = new Lazy<IDistributedCache>(
            () => new MemoryDistributedCache(
                new OptionsWrapper<MemoryDistributedCacheOptions>(
                    new MemoryDistributedCacheOptions())));
#else
        private static readonly Lazy<IDistributedCache> DefaultTokenCache = new Lazy<IDistributedCache>(
            () => new MemoryDistributedCache(
                new MemoryCache(
                    new OptionsWrapper<MemoryCacheOptions>(
                        new MemoryCacheOptions()))));
#endif

        /// <summary>
        /// Creates new ClientCredentialsGrantTokenClient
        /// </summary>
        /// <param name="tokenEndpointUri">OAuth2 Token endpoint URI</param>
        /// <param name="clientCredentials">OAuth2 client credentials</param>
        public ClientCredentialsGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials)
            : base(tokenEndpointUri, clientCredentials, DefaultTokenCache.Value)
        {
        }

        /// <summary>
        /// Creates new ClientCredentialsGrantTokenClient
        /// </summary>
        /// <param name="tokenEndpointUri">OAuth2 Token endpoint URI</param>
        /// <param name="clientCredentials">OAuth2 client credentials</param>
        /// <param name="cache">Token cache</param>
        public ClientCredentialsGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials, IDistributedCache cache)
            : base(tokenEndpointUri, clientCredentials, cache)
        {
        }

        /// <summary>
        /// Retrieve access token for the configured "client_id" and specified scopes. Request to the server is only performed if matching valid token is not in the cache
        /// </summary>
        /// <param name="scopes">OAuth2 scopes to request</param>
        /// <returns>Access token</returns>
        /// <exception cref="TokenEndpointException">Exception during token endpoint communication</exception>
        public Task<string> GetTokenAsync(params string[] scopes)
        {
            return GetTokenInternalAsync(new List<KeyValuePair<string, string>>(), scopes);
        }

        protected override string GrantType { get; } = "client_credentials";
    }
}
