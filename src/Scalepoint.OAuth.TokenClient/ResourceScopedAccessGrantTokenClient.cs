using System.Collections.Generic;
using System.Threading.Tasks;
using NameValuePair=System.Collections.Generic.KeyValuePair<string, string>;

namespace Scalepoint.OAuth.TokenClient
{
    /// <summary>
    /// OAuth2 Token endpoint client for "urn:scalepoint:params:oauth:grant-type:resource-scoped-access" grant
    /// </summary>
    public class ResourceScopedAccessGrantTokenClient : CustomGrantTokenClient
    {
        /// <summary>
        /// Creates new ResourceScopedAccessGrantTokenClient
        /// </summary>
        /// <param name="tokenEndpointUri">OAuth2 Token endpoint URI</param>
        /// <param name="clientCredentials">OAuth2 client credentials</param>
        public ResourceScopedAccessGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials)
            : base(tokenEndpointUri, clientCredentials, new NoCache())
        {
        }

        /// <summary>
        /// Retrieve access token for the configured "client_id", specified scope and resource
        /// </summary>
        /// <param name="parameters">Custom grant parameters</param>
        /// <returns>Access token</returns>
        /// <exception cref="TokenEndpointException">Exception during token endpoint communication</exception>
        public Task<string> GetTokenAsync(ResourceScopedAccessGrantParameters parameters)
        {
            return GetTokenInternalAsync(GetPostParams(parameters), parameters.Scope);
        }

        private IList<NameValuePair> GetPostParams(ResourceScopedAccessGrantParameters grantParameters)
        {
            var parameters = new List<NameValuePair>()
            {
                new NameValuePair("resource", grantParameters.Resource)
            };

            if (grantParameters.TenantId != null)
            {
                parameters.Add(new NameValuePair("tenantId", grantParameters.TenantId));
            }

            var amr = grantParameters.AmrString;
            if (amr != null)
            {
                parameters.Add(new NameValuePair("amr", amr));
            }

            return parameters;
        }

        protected override string GrantType => "urn:scalepoint:params:oauth:grant-type:resource-scoped-access";
    }
}