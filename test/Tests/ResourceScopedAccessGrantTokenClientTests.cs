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
                    TestCertificate.Load()
                ));
        }

        [Fact]
        public async void should_get_token()
        {
            var token = await _tokenClient.GetTokenAsync(new ResourceScopedAccessGrantParameters("test_scope", "test_resource"));
            Assert.NotNull(token);
        }
    }
}