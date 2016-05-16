using System.Collections.Generic;

namespace Scalepoint.OAuth.TokenClient
{
    /// <summary>
    /// OAuth2 client credentials
    /// </summary>
    public interface IClientCredentials
    {
        /// <summary>
        /// Client credentials token request parameters
        /// </summary>
        List<KeyValuePair<string, string>> PostParams { get; }

        /// <summary>
        /// Client credentials thumbprint value
        /// </summary>
        string CredentialThumbprint { get; }
    }
}
