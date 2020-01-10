using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Plutos
{
    class HelperMethods
    {
        public static string TrimString(string[] trimStrings, string value)
        {
            StringBuilder trimmedString = new StringBuilder(value);
            for (int i = 0; i < trimStrings.Length; i++)
            {
                trimmedString.Replace(trimStrings[i], String.Empty);
            }

            return trimmedString.ToString();
        }

        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ#?1234567890";

        public static string GetRandomString(int length)
        {
            StringBuilder randomString = new StringBuilder(String.Empty);
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider(valid))
            {
                while (randomString.Length < length)
                {
                    byte[] oneByte = new byte[1];
                    provider.GetBytes(oneByte);
                    char character = (char)oneByte[0];
                    if (valid.Contains(character))
                    {
                        randomString.Append(character);
                    }
                }
            }
            return randomString.ToString();
        }

        public static string GetDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy");
        }
    }
}
