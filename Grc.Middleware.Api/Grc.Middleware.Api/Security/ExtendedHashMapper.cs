using System.Security.Cryptography;
using System.Text;

namespace Grc.Middleware.Api.Security {
    public class ExtendedHashMapper {
        /// <summary>
        /// Hashes a password using SHA256 with a salt
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <param name="salt">Optional salt. If null, a random salt will be generated</param>
        /// <returns>A string containing the salt and hash separated by a colon</returns>
        public static string HashPassword(string password, string salt = null) {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            //..generate a random salt if none provided
            if (string.IsNullOrEmpty(salt)) {
                salt = GenerateRandomSalt();
            }

            //..combine password and salt
            string saltedPassword = $"{password}{salt}";

            //..create SHA256 hash
            using (SHA256 sha256 = SHA256.Create()) {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                string hash = Convert.ToBase64String(hashBytes);
            
                // Return salt:hash format for storage
                return $"{salt}:{hash}";
            }
        }

        /// <summary>
        /// Verifies a password against a stored hash
        /// </summary>
        /// <param name="password">The password to verify</param>
        /// <param name="storedHash">The stored hash in salt:hash format</param>
        /// <returns>True if password matches, false otherwise</returns>
        public static bool VerifyPassword(string password, string storedHash) {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
                return false;

            //..extract salt from stored hash
            string[] parts = storedHash.Split(':');
            if (parts.Length != 2)
                return false;

            string salt = parts[0];
            string expectedHash = parts[1];

            //..hash the provided password with the extracted salt
            string actualHashWithSalt = HashPassword(password, salt);
            string actualHash = actualHashWithSalt.Split(':')[1];
            return expectedHash == actualHash;
        }

        /// <summary>
        /// Generates a random salt for password hashing
        /// </summary>
        /// <returns>A random salt string</returns>
        private static string GenerateRandomSalt() {
            //..add 128-bit salt
            byte[] saltBytes = new byte[16]; 
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
    }
}
