using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;

namespace Tests
{
    public class MockServer
    {
        public static IDisposable Start(out int port)
        {
            port = GetFreePort();
            var url = $"http://localhost:{port}";
            return WebApp.Start<MockServerConfiguration>(url);
        }

        internal class MockServerConfiguration
        {
            public void Configuration(IAppBuilder app)
            {
                app.Map("/oauth/token", tokenApp =>
                {
                    tokenApp.Run(TokenEndpointHandler);
                });
            }

            private async Task TokenEndpointHandler(IOwinContext ctx)
            {
                await ctx.Response.WriteAsync("{\"access_token\":\"foo\", \"expires_in\": 3600}");
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