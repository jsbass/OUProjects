using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Portal.Helpers
{
    public class Hashing
    {
        private const int SALT_LENGTH = 20;
        private const int PASSWORD_HASH_LENGTH = 32;

        // Length of the base64 encoded salt
        // 20 bytes encoded in base64 = 4*ceil(20/3) = 28
        private const int ENCODED_SALT_LENGTH = 28;

        // Length of the hash output from SHA-256
        // 256 is 32 bytes
        // 256 bits encoded to base64 = 4*ceil(32/3) = 44
        private const int ENCODED_PASSWORD_HASH_LENGTH = 44;

        public static bool CheckPassword(string submitted, string storedHash)
        {
            var hashBytes = Convert.FromBase64String(storedHash);
            if (hashBytes.Length != PASSWORD_HASH_LENGTH+SALT_LENGTH) throw new Exception("decoded stored hash is not correct length");
            var salt = hashBytes.Skip(PASSWORD_HASH_LENGTH).ToArray();
            if(salt.Length != SALT_LENGTH) throw new Exception("decoded salt length is not correct length");
            var hash = Hash(Encoding.UTF8.GetBytes(submitted), salt);

            return storedHash == hash;
        }

        public static string GenerateHash(string password)
        {
            using (var rngeesus = new RNGCryptoServiceProvider())
            {
                var salt = new byte[SALT_LENGTH];
                rngeesus.GetBytes(salt);
                return Hash(Encoding.UTF8.GetBytes(password), salt);
            }
        }

        private static string Hash(byte[] password, byte[] salt)
        {
            using (var sha = new SHA256Managed())
            {
                var hash = sha.ComputeHash(password.Concat(salt).ToArray());

                return Convert.ToBase64String(hash.Concat(salt).ToArray());
            }
        }
    }
}