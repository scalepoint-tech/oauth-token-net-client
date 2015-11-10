using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OAuthJwtAssertionTokenClient
{
    public class JwtAssertionTokenClient
    {
        private readonly IReadThroughCache _cache;
        private static readonly Lazy<IReadThroughCache> DefaultTokenCache = new Lazy<IReadThroughCache>(() => new InMemoryReadThroughCache());
        private readonly PrivateKeyJwtClientCredentialsTokenClient _internalClient;
        private readonly string _cacheKey;

        public JwtAssertionTokenClient(string authorizationServerUrl, string clientId, X509Certificate2 certificate, string[] scopes)
            : this(authorizationServerUrl, clientId, certificate, scopes, DefaultTokenCache.Value)
        {
        }

        public JwtAssertionTokenClient(string authorizationServerUrl, string clientId, X509Certificate2 certificate, string[] scopes, IReadThroughCache cache)
        {
            _cache = cache;
            _internalClient = new PrivateKeyJwtClientCredentialsTokenClient(
                authorizationServerUrl,
                clientId,
                certificate,
                scopes);
            _cacheKey = string.Join("|", authorizationServerUrl, clientId, certificate.Thumbprint, string.Join("", scopes));
        }

        public async Task<string> GetToken()
        {
            return await _cache.GetAsync(_cacheKey, _internalClient.GetToken);
        }

    }
}
