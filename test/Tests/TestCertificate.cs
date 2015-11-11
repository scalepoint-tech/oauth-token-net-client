using System.Security.Cryptography.X509Certificates;

namespace Tests
{
    public class TestCertificate
    {
        public static X509Certificate2 Load()
        {
            return new X509Certificate2("client-test.pfx", "password");
        }
    }
}
