using System;
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
        [Obsolete("This property will be removed in the future. Please use Target property instead.")]
        public string Resource => Target;

        /// <summary>
        /// Specific target identifier
        /// </summary>
        public string Target { get; }

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
        /// <param name="target">Specific target identifier</param>
        public ResourceScopedAccessGrantParameters(string scope, string target)
        {
            Scope = scope;
            Target = target;
        }

        /// <summary>
        /// Creates new ResourceScopedAccessGrantParameters
        /// </summary>
        /// <param name="scope">OAuth2 scope</param>
        /// <param name="target">Specific target identifier</param>
        /// <param name="tenantId">Resource tenant identifier</param>
        /// <param name="amr">Original authentication method references</param>
        public ResourceScopedAccessGrantParameters(string scope, string target, string tenantId, IList<string> amr)
        {
            Scope = scope;
            Target = target;
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
