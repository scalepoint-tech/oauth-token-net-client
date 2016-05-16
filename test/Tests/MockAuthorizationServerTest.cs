using System;
using Scalepoint.OAuth.TokenClient;
using Xunit;

namespace Tests
{
    public class MockAuthorizationServerTest : IDisposable
    {
        private IDisposable _mockServer;
        private readonly ClientCredentialsGrantTokenClient _tokenClient;

        public MockAuthorizationServerTest()
        {
            int port;
            _mockServer = MockServer.Start(out port);
            var tokenEndpointUri = $"http://localhost:{port}/oauth/token";

            _tokenClient = new ClientCredentialsGrantTokenClient(
                tokenEndpointUri,
                new JwtBearerClientAssertionCredentials(
                    tokenEndpointUri,
                    "test_client",
                    TestCertificate.Load()
                ));
        }

        [Fact]
        public async void should_get_access_token()
        {
            var token = await _tokenClient.GetTokenAsync("test_scope");
            Assert.NotNull(token);
        }

        public void Dispose()
        {
            _mockServer.Dispose();
            _mockServer = null;
        }
    }
}
