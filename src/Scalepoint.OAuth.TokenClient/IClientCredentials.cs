using System.Collections.Generic;

namespace Scalepoint.OAuth.TokenClient
{
    public interface IClientCredentials
    {
        List<KeyValuePair<string, string>> PostParams { get; }

        string CredentialThumbprint { get; }
    }
}
