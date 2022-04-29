using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Tests
{
    public static class TestCertificate
    {
        private static readonly Lazy<X509Certificate2> TestCert = new Lazy<X509Certificate2>(GenerateCert);

        public static X509Certificate2 Get()
        {
            return TestCert.Value;
        }

        public static X509Certificate2 GenerateCert()
        {
            var rsa = RSA.Create();
            var req = new CertificateRequest($"cn=https://localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return req.CreateSelfSigned(DateTimeOffset.Now.AddMinutes(-1), DateTimeOffset.Now.AddYears(1));
        }
    }
}
