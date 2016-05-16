using System;
using System.Security.Cryptography.X509Certificates;

namespace Tests
{
    public static class TestCertificate
    {
        private static readonly Lazy<X509Certificate2> TestCert = new Lazy<X509Certificate2>(() => new X509Certificate2("client-test.pfx", "password"));

        public static X509Certificate2 Load()
        {
            return TestCert.Value;
        }
    }
}
