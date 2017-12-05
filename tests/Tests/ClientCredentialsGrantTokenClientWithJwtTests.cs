using System.Threading;
using System.Threading.Tasks;
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _tokenClient.Dispose();
            }
        }

        [Fact]
        public async Task should_cancel()
        {
            await Assert.ThrowsAsync<TaskCanceledException>(
                () => _tokenClient.GetTokenAsync(new[] { "test_scope" }, new CancellationToken(true)));
        }

        [Fact]
        public async Task should_complete_before_canceled()
        {
            using (var cts = new CancellationTokenSource(5000))
            {
                await _tokenClient.GetTokenAsync(new[] { "test_scope" }, cts.Token);
            }
        }

        [Fact]
        public async Task should_not_cancel()
        {
            await _tokenClient.GetTokenAsync(new[] { "test_scope" }, new CancellationToken(false));
        }
        [Fact]
        public async Task should_get_token()
        {
            var token = await _tokenClient.GetTokenAsync(new [] { "test_scope" });
            Assert.NotNull(token);
        }

        [Fact]
        public async Task should_get_token_from_cache()
        {
            var token1 = await _tokenClient.GetTokenAsync(new[] { "test_scope" });
            var token2 = await _tokenClient.GetTokenAsync(new[] { "test_scope" });
            Assert.Equal(token1, token2);
        }

        [Fact]
        public async Task should_handle_error()
        {
            await Assert.ThrowsAsync<TokenEndpointException>(async () =>
            {
                await _tokenClient.GetTokenAsync(new[] { "test_scope", "invalid_scope" });
            });
        }
    }
}
