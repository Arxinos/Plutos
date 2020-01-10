using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.IO;

namespace TCPClientTest
{
 //   [SerializableAttribute]
    class Message
    {
        byte[] mesByteArray;
        AesManaged aes;

        public Message(string message)
        {
            aes = new AesManaged();
            using (AesManaged aes = new AesManaged())// Creates a new member of AesManaged and with the member a completely random created key and initialization vector (IV)
            {
               // Console.WriteLine($"Encrypted string: {Encoding.UTF8.GetString(cipherText)}"); // $ = Interpolar String, makes the code easier to read. Everything written in {} will be executed as normal code

                mesByteArray = Encrypt(message, aes.Key, aes.IV);
            }
        }

        public static string CreateHash256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder(); // StringBuilder is used to boost perfomance, because the normal string class initializes a new string object every time when the string is modified
            byte[] crypto = crypt.ComputeHash(System.Text.Encoding.UTF8.GetBytes(randomString)); // .GetBytes creates a byte array of the string and .ComputeHash calculates the hash for delivered byte array
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x")); // X converts the raw byte into hexadecimal format, so we have readable characters instead of numbers only
            }
            return hash.ToString();
        }

        /// <summary>
        /// Encrypts a plaintext with a given key and Initialization vector (IV)
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }
        /// <summary>
        /// Decrypts a plaintext with a given key and Initialization vector (IV)
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}
