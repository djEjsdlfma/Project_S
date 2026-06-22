using System;
using System.Security.Cryptography;
using System.Text;

namespace Moon._01.Script.Datas
{
    public static class RealKey
    {
        private const string RKey = "U2seg1yS4x4D7h8Lnfdi8==";

        private static readonly byte[] FKey = 
        {
            31, 86, 5, 81, 33, 2, 30, 100, 65, 18, 97, 124, 68, 12, 5, 113
        };

        private static byte[] GetKey()
        {
            byte[] dummyBytes = Encoding.UTF8.GetBytes(RKey);
            byte[] realKey = new byte[FKey.Length];

            for (int i = 0; i < FKey.Length; i++)
            {
                realKey[i] = (byte)(FKey[i] ^ dummyBytes[i % dummyBytes.Length]);
            }

            return realKey;
        }

        public static string Encrypt(string text)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey();
                        
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

                byte[] resultBytes = new byte[aes.IV.Length + encryptedBytes.Length];
                        
                Buffer.BlockCopy(aes.IV, 0, resultBytes, 0, aes.IV.Length);
                Buffer.BlockCopy(encryptedBytes, 0, resultBytes, aes.IV.Length, encryptedBytes.Length);

                return Convert.ToBase64String(resultBytes);
            }
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedText);

            if (fullCipher.Length < 16) 
                return string.Empty;

            byte[] iv = new byte[16];
            byte[] cipherText = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, 16, cipherText, 0, cipherText.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKey();
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}