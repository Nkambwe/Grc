using System.Security.Cryptography;
using System.Text;

namespace Grc.Middleware.Api.Security {
    public static class HashGenerator {

        private static readonly string PASSKEY = "t0R4nGrcD4tAl0ad3RBym4rk";

        /// <summary>
        /// Encrypt a given message
        /// </summary>
        /// <param name="message">Message to encrypt</param>
        /// <returns></returns>
        public static string EncryptString(string message) {
            using (Aes aesAlg = Aes.Create()) {
                aesAlg.Key = SHA256.HashData(Encoding.UTF8.GetBytes(PASSKEY));
                aesAlg.Mode = CipherMode.CFB;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Generate IV
                aesAlg.GenerateIV();

                using ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using MemoryStream msEncrypt = new();
                // Write IV to the stream first
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new(csEncrypt)) {
                    swEncrypt.Write(message);
                }

                byte[] encryptedBytes = msEncrypt.ToArray();
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        /// <summary>
        /// Decrypts the given encrypted message using the provided
        /// </summary>
        /// <param name="encryptedMessage">Base64 encoded encrypted message</param>
        /// <returns>The decrypted string</returns>
        public static string DecryptString(string encryptedMessage) {
            byte[] fullCipher = Convert.FromBase64String(encryptedMessage);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = SHA256.HashData(Encoding.UTF8.GetBytes(PASSKEY));
            aesAlg.Mode = CipherMode.CFB;
            aesAlg.Padding = PaddingMode.PKCS7;

            byte[] iv = new byte[aesAlg.BlockSize / 8];
            byte[] cipherText = new byte[fullCipher.Length - iv.Length];

            // Extract IV from the beginning of the ciphertext
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

            using ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);
            using MemoryStream msDecrypt = new(cipherText);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }

        public static string GenerateHashedPassword(string password, int length) {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new();
            foreach (byte b in bytes) {
                builder.Append(b.ToString("x2"));
            }

            string baseHash = builder.ToString()[..(length - 4)];
            string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            StringBuilder finalPassword = new(baseHash);

            for (int i = 0; i < 4; i++) {
                int charIndex = RandomNumberGenerator.GetInt32(specialChars.Length);
                int insertIndex = RandomNumberGenerator.GetInt32(finalPassword.Length + 1);

                finalPassword.Insert(insertIndex, specialChars[charIndex]);
            }

            return finalPassword.ToString();
        }
    }
}
