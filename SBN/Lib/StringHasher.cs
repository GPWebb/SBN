using System;
using System.Security.Cryptography;
using System.Text;

namespace SBN.Lib
{
    public class StringHasher : IStringHasher
    {
        public string HashString(string input)
        {
            using (var algorithm = SHA512.Create())
            {
                var hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
