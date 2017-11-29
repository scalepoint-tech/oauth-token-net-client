using System;

namespace Tests
{
    public class MockServerTestBase : IDisposable
    {
        protected string TokenEndpointUri { get; }
        private readonly IDisposable _mockServer;

        public MockServerTestBase()
        {
            _mockServer = MockServer.Start(out var tokenEndpointUri);
            TokenEndpointUri = tokenEndpointUri;
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
