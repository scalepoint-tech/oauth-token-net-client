using System.Security.Cryptography.X509Certificates;
using Scalepoint.OAuth.TokenClient.Internals;
using Xunit;

namespace Tests
{
    public class ClientAssertionJwtFactoryTests
    {
        private readonly string _tokenEndpointUri;
        private readonly string _clientId;
        private readonly X509Certificate2 _certificate;
        private readonly ClientAssertionJwtFactory _factory;

        public ClientAssertionJwtFactoryTests()
        {
            _tokenEndpointUri = "https://localhost/oauth2/token";
            _clientId = "test_client";
            _certificate = TestCertificate.Get();
            _factory = new ClientAssertionJwtFactory(_tokenEndpointUri, _clientId, _certificate);
        }

        [Fact]
        public void should_create_valid_token()
        {
            var tokenString = _factory.CreateAssertionToken();

            TestAssertionValidator.Validate(
                tokenString,
                _tokenEndpointUri,
                _clientId,
                _certificate
            );
        }
    }
}
