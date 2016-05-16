using Scalepoint.OAuth.TokenClient;
using Xunit;

namespace Tests
{
    public class RealAuthorizationServerTest
    {
        private readonly ClientCredentialsGrantTokenClient _tokenClient;

        public RealAuthorizationServerTest()
        {
            var tokenEndpointUri = "https://localhost:44300/connect/token";

            _tokenClient = new ClientCredentialsGrantTokenClient(
                tokenEndpointUri,
                new JwtBearerClientAssertionCredentials(
                    tokenEndpointUri,
                    "test_client",
                    TestCertificate.Load()
                ));

        }

        [Fact(Skip = "Disable until server side implementation is released in upstream so that it can be used with NuGet")]
        public async void should_get_access_token()
        {
            var token = await _tokenClient.GetTokenAsync("test_scope");
            Assert.NotNull(token);
        }
    }
}
