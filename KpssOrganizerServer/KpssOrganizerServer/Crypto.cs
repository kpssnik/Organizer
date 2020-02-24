using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KpssOrganizerServer
{
    public static class Crypto
    {
        public static byte[] Key = Encoding.ASCII.GetBytes("fuckingCourseach");
        public static byte[] IV = Encoding.ASCII.GetBytes("organizerEpta4to");

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));

                return builder.ToString();
            }
        }

        public static byte[] SimEncrypt(string plainText)
        {
            if (plainText == null || plainText.Length <= 0)

                throw new ArgumentNullException("plainText");
            byte[] encrypted;
            using (Rijndael rijAlg = Rijndael.Create())
            {
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(Key, IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }
        public static string SimDecrypt(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            string plaintext = null;

            using (Rijndael rijAlg = Rijndael.Create())
            {
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(Key, IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
