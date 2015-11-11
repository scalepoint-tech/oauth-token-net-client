using System;
using System.Runtime.Serialization;

namespace OAuthJwtAssertionTokenClient
{
    /// <summary>
    /// Represents OAuth2 token endpoint error
    /// </summary>
    public class TokenEndpointException : Exception
    {
        public TokenEndpointException()
        {
        }

        public TokenEndpointException(string message) : base(message)
        {
        }

        public TokenEndpointException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TokenEndpointException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
