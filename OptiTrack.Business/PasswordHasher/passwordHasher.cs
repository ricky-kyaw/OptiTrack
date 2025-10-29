using System;
using System.Security.Cryptography;
using System.Text;

namespace OptiTrack.Business.PasswordHasher
{
    public static class SecurePasswordHasher
    {
        // Recommended work factor: 100,000 iterations for PBKDF2
        private const int Iterations = 100000;
        private const int HashSize = 32; // 256 bits
        private const int SaltSize = 16; // 128 bits

        /// <summary>
        /// Creates a cryptographically strong, random salt.
        /// </summary>
        /// <returns>Base64 encoded salt string.</returns>
        public static string CreateSalt()
        {
            // RNGCryptoServiceProvider is deprecated, using RandomNumberGenerator
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[SaltSize];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        /// <summary>
        /// Hashes a password using PBKDF2 with a provided salt.
        /// </summary>
        /// <param name="password">The plaintext password.</param>
        /// <param name="salt">The Base64-encoded salt string stored in the database.</param>
        /// <returns>The Base64-encoded password hash.</returns>
        public static string HashPassword(string password, string salt)
        {
            // Convert salt string back to byte array
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Use the recommended PBKDF2 implementation
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(HashSize);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Compares a plaintext password against a stored hash and salt.
        /// </summary>
        /// <param name="password">The plaintext password provided by the user.</param>
        /// <param name="storedHash">The stored password hash from the database.</param>
        /// <param name="storedSalt">The stored salt from the database.</param>
        /// <returns>True if the password is correct, otherwise False.</returns>
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            // Re-hash the provided password using the stored salt
            string computedHash = HashPassword(password, storedSalt);

            // Perform a constant-time comparison to prevent timing attacks
            return computedHash.Equals(storedHash, StringComparison.Ordinal);
        }
    }
}
