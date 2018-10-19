using System;
using System.Runtime.Serialization;

namespace Scalepoint.OAuth.TokenClient
{
    /// <summary>
    /// Represents OAuth2 token endpoint error
    /// </summary>
    [Serializable]
    public class TokenEndpointException : Exception
    {
        // ReSharper disable once UnusedMember.Global
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
