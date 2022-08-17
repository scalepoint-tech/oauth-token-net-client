using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NameValuePair=System.Collections.Generic.KeyValuePair<string, string>;

namespace Scalepoint.OAuth.TokenClient
{
    /// <summary>
    /// OAuth2 Token endpoint client for "urn:scalepoint:params:oauth:grant-type:resource-scoped-access" grant
    /// </summary>
    public sealed class ResourceScopedAccessGrantTokenClient : IDisposable
    {
        private readonly CustomGrantTokenClient _customGrantTokenClient;
        /// <summary>
        /// Creates new ResourceScopedAccessGrantTokenClient
        /// </summary>
        /// <param name="tokenEndpointUri">OAuth2 Token endpoint URI</param>
        /// <param name="clientCredentials">OAuth2 client credentials</param>
        public ResourceScopedAccessGrantTokenClient(string tokenEndpointUri, IClientCredentials clientCredentials)
        {
            _customGrantTokenClient = new CustomGrantTokenClient(tokenEndpointUri, clientCredentials, "urn:scalepoint:params:oauth:grant-type:resource-scoped-access");
        }

        /// <summary>
        /// Retrieve access token for the configured "client_id", specified scope and resource
        /// </summary>
        /// <param name="parameters">Custom grant parameters</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Access token</returns>
        /// <exception cref="TokenEndpointException">Exception during token endpoint communication</exception>
        public async Task<string> GetTokenAsync(ResourceScopedAccessGrantParameters parameters, CancellationToken token = default(CancellationToken))
        {
            var result = await _customGrantTokenClient.GetTokenInternalAsync(GetPostParams(parameters), new [] { parameters.Scope }, token).ConfigureAwait(false);
            return result.Item1;
        }

        private IList<NameValuePair> GetPostParams(ResourceScopedAccessGrantParameters grantParameters)
        {
            var parameters = new List<NameValuePair>()
            {
                new NameValuePair("sso_resource", grantParameters.Resource)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ResourceScopedAccessGrantTokenClient()
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
                _disposed = true;
            }
        }
    }
}
