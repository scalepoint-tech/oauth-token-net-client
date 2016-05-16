using System;

namespace Tests
{
    public class MockServerTestBase : IDisposable
    {
        protected string TokenEndpointUri { get; }
        private IDisposable _mockServer;

        public MockServerTestBase()
        {
            string tokenEndpointUri;
            _mockServer = MockServer.Start(out tokenEndpointUri);
            TokenEndpointUri = tokenEndpointUri;
        }

        public void Dispose()
        {
            _mockServer.Dispose();
            _mockServer = null;
        }
    }
}