using Scalepoint.OAuth.TokenClient;
using Xunit;

namespace Tests
{
    public class ClientCredentialsGrantTokenClientWithJwtTests : MockServerTestBase
    {
        private readonly ClientCredentialsGrantTokenClient _tokenClient;

        public ClientCredentialsGrantTokenClientWithJwtTests()
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

        [Fact]
        public async void should_get_token_from_cache()
        {
            var token1 = await _tokenClient.GetTokenAsync("test_scope");
            var token2 = await _tokenClient.GetTokenAsync("test_scope");
            Assert.Equal(token1, token2);
        }

        [Fact]
        public async void should_handle_error()
        {
            await Assert.ThrowsAsync<TokenEndpointException>(async () =>
            {
                await _tokenClient.GetTokenAsync("test_scope", "invalid_scope");
            });
        }
    }
}
