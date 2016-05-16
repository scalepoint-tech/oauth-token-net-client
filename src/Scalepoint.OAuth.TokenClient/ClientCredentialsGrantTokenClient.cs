using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scalepoint.OAuth.TokenClient
{
    public class ClientCredentialsGrantTokenClient : CustomGrantTokenClient
    {
        private static readonly Lazy<ITokenCache> DefaultTokenCache = new Lazy<ITokenCache>(() => new InMemoryTokenCache());

        public ClientCredentialsGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials)
            : base(tokenEndpointUri, clientCredentials, DefaultTokenCache.Value)
        {
        }

        public ClientCredentialsGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials, ITokenCache cache)
            : base(tokenEndpointUri, clientCredentials, cache)
        {
        }

        public Task<string> GetToken(params string[] scopes)
        {
            return GetTokenInternal(Enumerable.Empty<KeyValuePair<string, string>>(), scopes);
        }

        protected override string GrantType { get; } = "client_credentials";
    }
}