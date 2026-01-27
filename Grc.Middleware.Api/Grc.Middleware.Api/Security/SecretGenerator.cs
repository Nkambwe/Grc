using System.Security.Cryptography;

namespace Grc.Middleware.Api.Security {

    public sealed class SecurePasswordGenerator {
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits    = "0123456789";
        private const string Special   = "!@#$%^&*()-_=+[]{}<>?";

        private static readonly string AllCharacters =
            Uppercase + Lowercase + Digits + Special;

        public static string Generate(int length = 8) {
            if (length < 4)
                throw new ArgumentException("Password length must be at least 4.");

            char[] password = new char[length];
            int index = 0;

            password[index++] = GetRandomChar(Uppercase);
            password[index++] = GetRandomChar(Lowercase);
            password[index++] = GetRandomChar(Special);
            password[index++] = GetRandomChar(Digits);

            for (; index < length; index++){
                password[index] = GetRandomChar(AllCharacters);
            }

            // Secure shuffle using CSPRNG
            Shuffle(password);
            return new string(password);
        }

        private static char GetRandomChar(string source)
            => source[RandomNumberGenerator.GetInt32(source.Length)];

        private static void Shuffle(char[] array) {
            for (int i = array.Length - 1; i > 0; i--) {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
    }

}
