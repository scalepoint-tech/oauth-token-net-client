using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scalepoint.OAuth.TokenClient;
using Xunit;

namespace Tests
{
    public class ResourceScopedAccessGrantTokenClientTests : MockServerTestBase
    {
        private readonly ResourceScopedAccessGrantTokenClient _tokenClient;

        public ResourceScopedAccessGrantTokenClientTests()
        {
            _tokenClient = new ResourceScopedAccessGrantTokenClient(
                TokenEndpointUri,
                new JwtBearerClientAssertionCredentials(
                    TokenEndpointUri,
                    "test_client",
                    TestCertificate.Get()
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
                () => _tokenClient.GetTokenAsync(new ResourceScopedAccessGrantParameters("test_scope", "test_resource"), new CancellationToken(true)));
        }

        [Fact]
        public async Task should_complete_before_canceled()
        {
            using (var cts = new CancellationTokenSource(5000))
            {
                await _tokenClient.GetTokenAsync(new ResourceScopedAccessGrantParameters("test_scope", "test_resource"), cts.Token);
            }
        }

        [Fact]
        public async Task should_not_cancel()
        {
            await _tokenClient.GetTokenAsync(new ResourceScopedAccessGrantParameters("test_scope", "test_resource"), new CancellationToken(false));
        }

        [Fact]
        public async Task should_get_token()
        {
            var token = await _tokenClient.GetTokenAsync(new ResourceScopedAccessGrantParameters("test_scope", "test_resource"));
            Assert.NotNull(token);
        }

        [Fact]
        public async Task should_get_token_with_optional_parameters()
        {
            var token = await _tokenClient.GetTokenAsync(
                new ResourceScopedAccessGrantParameters(
                    "test_scope",
                    "test_resource",
                    "test_tenant",
                    new List<string>() {"pwd", "otp", "mfa"}
                ));
            Assert.NotNull(token);
        }

        [Fact]
        public async Task should_handle_error()
        {
            await Assert.ThrowsAsync<TokenEndpointException>(async () =>
            {
                await _tokenClient.GetTokenAsync(new ResourceScopedAccessGrantParameters("test_scope", "invalid_resource"));
            });
        }
    }
}
