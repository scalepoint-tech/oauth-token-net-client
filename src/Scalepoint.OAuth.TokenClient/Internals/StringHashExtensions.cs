using System;
using System.Security.Cryptography;
using System.Text;

namespace Scalepoint.OAuth.TokenClient.Internals
{
    internal static class StringHashExtensions
    {
        public static string Sha1Hex(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // SHA1 is used for thumbprints
#pragma warning disable CA5350 // Do not use insecure cryptographic algorithm SHA1.
            using (var sha = SHA1.Create())
#pragma warning restore CA5350 // Do not use insecure cryptographic algorithm SHA1.
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }
}
