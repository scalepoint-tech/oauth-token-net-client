using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Tests
{
    public static class MockServer
    {
        public static IDisposable Start(out string tokenEndpointUri)
        {
            var port = GetFreePort();
            var uri = $"http://localhost:{port}";
            tokenEndpointUri = $"{uri}/oauth2/token";

            var host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseUrls(uri)
                        .Configure(app =>
                            {
                                app.Map("/oauth2/token", tokenApp => { tokenApp.Run(TokenEndpointHandler); });
                            }
                        );
                })
                .Build();
            host.Start();
            return host;
        }

        private static async Task TokenEndpointHandler(HttpContext ctx)
        {
            int tokenId = 0;
            if (await Validate(ctx.Request).ConfigureAwait(false))
            {
                var tid = Interlocked.Increment(ref tokenId);
                await ctx.Response.WriteAsync("{\"access_token\":\"" + tid + "\", \"expires_in\": 3600}")
                    .ConfigureAwait(false);
            }
            else
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsync("{\"error\":\"error\"}").ConfigureAwait(false);
            }
        }

        private static async Task<bool> Validate(HttpRequest request)
        {
            var form = await request.ReadFormAsync().ConfigureAwait(false);

            // Authentication
            if (form["client_assertion_type"] == "urn:ietf:params:oauth:client-assertion-type:jwt-bearer")
            {
                if (!ValidateAssertion(form["client_assertion"], request.Host)) return false;
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
                if (form["target"] != "test_resource") return false;
                if (form["tenantId"] != (string) null && form["tenantId"] != "test_tenant") return false;
                if (form["amr"] != (string) null && form["amr"] != "pwd otp mfa") return false;
            }

            return true;
        }

        private static bool ValidateAssertion(string assertion, HostString host)
        {
            try
            {
                TestAssertionValidator.Validate(
                    assertion,
                    $"http://{host}/oauth2/token",
                    "test_client",
                    TestCertificate.Get()
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static int GetFreePort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
