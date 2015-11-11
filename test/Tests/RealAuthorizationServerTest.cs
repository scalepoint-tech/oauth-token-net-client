using OAuthJwtAssertionTokenClient;
using Xunit;

namespace Tests
{
    public class RealAuthorizationServerTest
    {
        private readonly JwtAssertionTokenClient _tokenClient;

        public RealAuthorizationServerTest()
        {
            _tokenClient = new JwtAssertionTokenClient("https://localhost:44300/connect/token", "test_client", TestCertificate.Load(), new[] { "test_scope" });
        }

        [Fact]
        public async void should_get_access_token()
        {
            var token = await _tokenClient.GetToken();
            Assert.NotNull(token);
        }
    }
}
