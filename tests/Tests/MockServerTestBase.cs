using System;
using Microsoft.AspNetCore.TestHost;

namespace Tests
{
    public class MockServerTestBase : IDisposable
    {
        protected string TokenEndpointUri { get; }
        private readonly IDisposable _mockServer;

        public MockServerTestBase()
        {
            string _tokenEndpointUri;
            _mockServer = MockServer.Start(out _tokenEndpointUri);
            TokenEndpointUri = _tokenEndpointUri;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mockServer?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MockServerTestBase()
        {
            Dispose(false);
        }
    }
}
