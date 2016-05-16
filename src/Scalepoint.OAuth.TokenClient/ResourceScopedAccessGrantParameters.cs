using System.Collections.Generic;

namespace Scalepoint.OAuth.TokenClient
{
    /// <summary>
    /// OAuth2 Token endpoint parameters for "urn:scalepoint:params:oauth:grant-type:resource-scoped-access" grant
    /// </summary>
    public class ResourceScopedAccessGrantParameters
    {
        /// <summary>
        /// OAuth2 scope
        /// </summary>
        public string Scope { get; }

        /// <summary>
        /// Specific resource identifier
        /// </summary>
        public string Resource { get; }

        /// <summary>
        /// Resource tenant identifier
        /// </summary>
        public string TenantId { get; }

        /// <summary>
        /// Original authentication method references
        /// </summary>
        public IList<string> Amr { get; }

        /// <summary>
        /// Creates new ResourceScopedAccessGrantParameters
        /// </summary>
        /// <param name="scope">OAuth2 scope</param>
        /// <param name="resource">Specific resource identifier</param>
        public ResourceScopedAccessGrantParameters(string scope, string resource)
        {
            Scope = scope;
            Resource = resource;
        }

        /// <summary>
        /// Creates new ResourceScopedAccessGrantParameters
        /// </summary>
        /// <param name="scope">OAuth2 scope</param>
        /// <param name="resource">Specific resource identifier</param>
        /// <param name="tenantId">Resource tenant identifier</param>
        /// <param name="amr">Original authentication method references</param>
        public ResourceScopedAccessGrantParameters(string scope, string resource, string tenantId, IList<string> amr)
        {
            Scope = scope;
            Resource = resource;
            TenantId = tenantId;
            Amr = amr;
        }

        /// <summary>
        /// String-joined authentication method references
        /// </summary>
        public string AmrString =>
            (Amr == null || Amr.Count < 1)
                ? null
                : string.Join(" ", Amr);
    }
}