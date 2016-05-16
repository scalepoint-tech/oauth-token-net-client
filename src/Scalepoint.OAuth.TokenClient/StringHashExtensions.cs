using System;
using System.Security.Cryptography;
using System.Text;

namespace Scalepoint.OAuth.TokenClient
{
    internal static class StringHashExtensions
    {
        public static string Sha1Hex(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            using (var sha = SHA1.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }
}