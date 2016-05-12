using System.Security.Cryptography.X509Certificates;

namespace OAuthJwtAssertionTokenClient
{
    /// <summary>
    /// Token client options
    /// </summary>
    public class TokenClientOptions
    {
        /// <summary>
        /// Token endpoint Url
        /// </summary>
        public string TokenEndpointUrl { get; set; }

        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// JWT signing certificate
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Access token cache
        /// </summary>
        public ITokenCache Cache { get; set; }

        /// <summary>
        /// JWT "aud" value. If not set, defaults to <see cref="TokenEndpointUrl"/>
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// When set to true, a full certificate will be added to the token header in "x5c"
        /// </summary>
        public bool EmbedCertificate { get; set; } = false;
    }
}