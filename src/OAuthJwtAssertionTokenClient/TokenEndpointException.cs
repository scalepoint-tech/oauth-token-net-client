using System;
using System.Runtime.Serialization;

namespace OAuthJwtAssertionTokenClient
{
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
