using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;

namespace Tests
{
    public class MockServer
    {
        public static IDisposable Start(out string tokenEndpointUri)
        {
            var port = GetFreePort();
            var uri = $"http://localhost:{port}";
            tokenEndpointUri = $"{uri}/oauth2/token";
            return WebApp.Start(uri, new MockServerConfiguration(tokenEndpointUri).Configuration);
        }

        internal class MockServerConfiguration
        {
            private readonly string _tokenEndpointUri;
            private int _tokenId = 0;

            public MockServerConfiguration(string tokenEndpointUri)
            {
                _tokenEndpointUri = tokenEndpointUri;
            }

            public void Configuration(IAppBuilder app)
            {
                app.Map("/oauth2/token", tokenApp =>
                {
                    tokenApp.Run(TokenEndpointHandler);
                });
            }

            private async Task TokenEndpointHandler(IOwinContext ctx)
            {
                if (await Validate(ctx.Request))
                {
                    var tokenId = Interlocked.Increment(ref _tokenId);
                    await ctx.Response.WriteAsync("{\"access_token\":\""+tokenId+"\", \"expires_in\": 3600}");
                }
                else
                {
                    ctx.Response.StatusCode = 400;
                    await ctx.Response.WriteAsync("{\"error\":\"error\"}");
                }

            }

            private async Task<bool> Validate(IOwinRequest request)
            {
                var form = await request.ReadFormAsync();

                // Authentication
                if (form["client_assertion_type"] == "urn:ietf:params:oauth:client-assertion-type:jwt-bearer")
                {
                    if (!ValidateAssertion(form["client_assertion"])) return false;
                }
                else
                {
                    if (form["client_id"] != "test_client") return false;
                    if (form["client_secret"] != "test_secret") return false;
                }

                // Scope & grant-specific
                if (form["grant_type"] != "client_credentials" &&
                    form["grant_type"] != "urn:scalepoint:params:oauth:grant-type:resource-scoped-access")
                {
                    return false;
                }

                if (form["scope"] != "test_scope") return false;

                if (form["grant_type"] == "urn:scalepoint:params:oauth:grant-type:resource-scoped-access")
                {
                    if (form["resource"] != "test_resource") return false;
                    if (form["tenantId"] != null && form["tenantId"] != "test_tenant") return false;
                    if (form["amr"] != null && form["amr"] != "pwd otp mfa") return false;
                }

                return true;
            }

            private bool ValidateAssertion(string assertion)
            {
                return TestAssertionValidator.Validate(
                    assertion,
                    _tokenEndpointUri,
                    "test_client",
                    TestCertificate.Load()
                );
            }
        }

        private static int GetFreePort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}