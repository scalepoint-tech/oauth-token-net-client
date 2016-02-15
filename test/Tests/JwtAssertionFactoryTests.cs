using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using OAuthJwtAssertionTokenClient;
using Xunit;

namespace Tests
{
    public class JwtAssertionFactoryTests
    {
        private readonly string _tokenEndpointUrl;
        private readonly string _clientId;
        private readonly X509Certificate2 _certificate;
        private readonly JwtAssertionFactory _factory;

        public JwtAssertionFactoryTests()
        {
            _tokenEndpointUrl = "https://authorizationserver.test/oauth2/token";
            _clientId = "test_client";
            _certificate = TestCertificate.Load();
            _factory = new JwtAssertionFactory(new TokenClientOptions()
            {
                TokenEndpointUrl = _tokenEndpointUrl,
                ClientId = _clientId,
                Certificate = _certificate
            });
        }

        [Fact]
        public void should_create_valid_token()
        {
            var tokenString = _factory.CreateAssertionToken();

            var validationParameters = new TokenValidationParameters()
            {
                ValidIssuer = _clientId,
                ValidateIssuer = true,

                ValidAudience = _tokenEndpointUrl,
                ValidateAudience = true,

                IssuerSigningKey = new X509AsymmetricSecurityKey(_certificate),
                ValidateIssuerSigningKey = true,

                RequireSignedTokens = true,
                RequireExpirationTime = true
            };

            SecurityToken token;
            new JwtSecurityTokenHandler().ValidateToken(tokenString, validationParameters, out token);

            var jwt = (JwtSecurityToken) token;

            Assert.Equal(jwt.Header.Alg, JwtAlgorithms.RSA_SHA256);
            Assert.Equal(jwt.Subject, _clientId);
            Assert.Single(jwt.Claims, c => c.Type == JwtClaimTypes.JwtId);
        }
    }
}
