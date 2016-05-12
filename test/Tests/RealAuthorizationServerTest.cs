using Scalepoint.OAuth.TokenClient;
using Xunit;

namespace Tests
{
    public class RealAuthorizationServerTest
    {
        private readonly JwtAssertionTokenClient _tokenClient;

        public RealAuthorizationServerTest()
        {
            var options = new TokenClientOptions()
            {
                TokenEndpointUrl = "https://localhost:44300/connect/token",
                ClientId = "test_client",
                Certificate = TestCertificate.Load()
            };

            _tokenClient = new JwtAssertionTokenClient(options);
        }

        [Fact(Skip = "Disable until server side implementation is released in upstream so that it can be used with NuGet")]
        public async void should_get_access_token()
        {
            var token = await _tokenClient.GetAccessTokenAsync("test_scope");
            Assert.NotNull(token);
        }
    }
}
