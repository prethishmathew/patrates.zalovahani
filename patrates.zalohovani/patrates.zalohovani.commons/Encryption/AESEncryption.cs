using patrates.zalohovani.interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace patrates.zalohovani.commons
{
    public class AESEncryption: ICryption
    {
        private static readonly byte[] initVectorBytes = Encoding.ASCII.GetBytes("tu89geji340t89u2");
        private const int keysize = 256;

        public string encrypt(string data)
        {
            if (string.IsNullOrEmpty(data)) {return null;}
            string encrypt;
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(data);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(_key, null))
            {
                byte[] keyBytes = password.GetBytes(keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] cipherTextBytes = memoryStream.ToArray();
                                encrypt = Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }

            return encrypt;

        }

        public string decrypt(string data)
        {
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;
            ICryptoTransform decryptor = null;
            RijndaelManaged symmetricKey = null;
            try
            {

            
            if (string.IsNullOrEmpty(data)) { return null; }
            string decrypt;
            byte[] cipherTextBytes = Convert.FromBase64String(data);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(_key, null))
            {
                byte[] keyBytes = password.GetBytes(keysize / 8);
                using (symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                    {
                        memoryStream = new MemoryStream(cipherTextBytes);
                        {
                            using (cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {             
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                decrypt = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
            return decrypt;
            }
            finally
            {

               
            }

            

        }

        private string _key = "T0!am1aM^t2Z1%8L";
        public string key
        {

            get { return null; }
            set { _key = value; }
        }
    }
}
