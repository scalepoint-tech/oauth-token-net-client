using Scalepoint.OAuth.TokenClient;
using Xunit;

namespace Tests
{
    public class ClientCredentialsGrantTokenClientTests : MockServerTestBase
    {
        private readonly ClientCredentialsGrantTokenClient _tokenClient;

        public ClientCredentialsGrantTokenClientTests()
        {
            _tokenClient = new ClientCredentialsGrantTokenClient(
                TokenEndpointUri,
                new JwtBearerClientAssertionCredentials(
                    TokenEndpointUri,
                    "test_client",
                    TestCertificate.Load()
                ));
        }

        [Fact]
        public async void should_get_token()
        {
            var token = await _tokenClient.GetTokenAsync("test_scope");
            Assert.NotNull(token);
        }
    }
}
