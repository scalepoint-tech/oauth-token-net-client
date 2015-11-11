using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OAuthJwtAssertionTokenClient
{
    /// <summary>
    /// Token client that authenticates using "private_key_jwt" authentication method
    /// </summary>
    public class JwtAssertionTokenClient
    {
        private readonly ITokenCache _cache;
        private static readonly Lazy<ITokenCache> DefaultTokenCache = new Lazy<ITokenCache>(() => new InMemoryTokenCache());
        private readonly PrivateKeyJwtClientCredentialsTokenClient _internalClient;
        private readonly string _cacheKey;

        /// <summary>
        /// Constructs a JwtAssertionTokenClient with default (in-memory) cache
        /// </summary>
        /// <param name="authorizationServerUrl">Authorization Server token endpoint</param>
        /// <param name="clientId">OAuth2 client_id</param>
        /// <param name="certificate">Certificate used for signing JWT client assertion (must have private key)</param>
        /// <param name="scopes">OAuth2 scopes</param>
        public JwtAssertionTokenClient(string authorizationServerUrl, string clientId, X509Certificate2 certificate, string[] scopes)
            : this(authorizationServerUrl, clientId, certificate, scopes, DefaultTokenCache.Value)
        {
        }

        /// <summary>
        /// Constructs a JwtAssertionTokenClient with default (in-memory) cache
        /// </summary>
        /// <param name="authorizationServerUrl">Authorization Server token endpoint</param>
        /// <param name="clientId">OAuth2 client_id</param>
        /// <param name="certificate">Certificate used for signing JWT client assertion (must have private key)</param>
        /// <param name="scopes">OAuth2 scopes</param>
        /// <param name="cache">Token cache</param>
        public JwtAssertionTokenClient(string authorizationServerUrl, string clientId, X509Certificate2 certificate, string[] scopes, ITokenCache cache)
        {
            ValidateCertificate(certificate);
            _cache = cache;
            _internalClient = new PrivateKeyJwtClientCredentialsTokenClient(
                authorizationServerUrl,
                clientId,
                certificate,
                scopes);
            _cacheKey = string.Join("|", authorizationServerUrl, clientId, certificate.Thumbprint, string.Join("", scopes));
        }

        private static void ValidateCertificate(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentException("Certificate is required", nameof(certificate));
            }
            if (!certificate.HasPrivateKey)
            {
                throw new ArgumentException("Certificate has no private key and cannot be used for token signing", nameof(certificate));
            }
            try
            {
                // ReSharper disable once UnusedVariable
                var pk = certificate.PrivateKey;
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    "Certificate has a private key, but it cannot be accessed. " +
                    "Either user account has no permission to access private key or this is a CNG certificate. " +
                    "Only CSP certificates are fully supported by X509Certificate2.",
                    nameof(certificate),
                    e);
            }
        }

        public async Task<string> GetToken()
        {
            return await _cache.GetAsync(_cacheKey, _internalClient.GetToken);
        }

    }
}
