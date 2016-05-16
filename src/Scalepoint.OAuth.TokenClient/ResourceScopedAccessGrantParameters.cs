using System.Collections.Generic;

namespace Scalepoint.OAuth.TokenClient
{
    public class ResourceScopedAccessGrantParameters
    {
        public string Scope { get; }
        public string Resource { get; }
        public string TenantId { get; }
        public IList<string> Amr { get; }

        public ResourceScopedAccessGrantParameters(string scope, string resource)
        {
            Scope = scope;
            Resource = resource;
        }

        public ResourceScopedAccessGrantParameters(string scope, string resource, string tenantId, IList<string> amr)
        {
            Scope = scope;
            Resource = resource;
            TenantId = tenantId;
            Amr = amr;
        }

        public string AmrString =>
            (Amr == null || Amr.Count < 1)
                ? null
                : string.Join(" ", Amr);
    }
}