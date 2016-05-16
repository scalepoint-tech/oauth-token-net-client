using System.Collections.Generic;
using System.Threading.Tasks;
using NameValuePair=System.Collections.Generic.KeyValuePair<string, string>;

namespace Scalepoint.OAuth.TokenClient
{
    public class ResourceScopedAccessGrantTokenClient : CustomGrantTokenClient
    {
        public ResourceScopedAccessGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials)
            : base(tokenEndpointUri, clientCredentials, new NoCache())
        {
        }

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