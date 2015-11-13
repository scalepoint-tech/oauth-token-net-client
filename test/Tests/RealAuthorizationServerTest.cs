using OAuthJwtAssertionTokenClient;
using Xunit;

namespace Tests
{
    public class RealAuthorizationServerTest
    {
        private readonly JwtAssertionTokenClient _tokenClient;

        public RealAuthorizationServerTest()
        {
            _tokenClient = new JwtAssertionTokenClient("https://localhost:44300/connect/token", "test_client", TestCertificate.Load());
        }

        [Fact(Skip = "Disable until server side implementation is released in upstream so that it can be used with NuGet")]
        public async void should_get_access_token()
        {
            var token = await _tokenClient.GetAccessTokenAsync("test_scope");
            Assert.NotNull(token);
        }
    }
}
