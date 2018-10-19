using System.Collections.Generic;

namespace Scalepoint.OAuth.TokenClient.Internals
{
    public static class ScopeHelper
    {
        public static string ToScopeString(IEnumerable<string> scopes)
        {
            return scopes != null
                ? string.Join(" ", scopes)
                : null;
        }
    }
}
