using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;

public class RSAManager
{
    #region Keys, Containername, Keysizes
    public RSAParameters publicKey;
    public RSAParameters privateKey;
    #endregion

    #region Methods
    public RSAManager()
    {
        GenerateKeys();
    }
    
    public void GenerateKeys()
    {
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            rsa.PersistKeyInCsp = false; //Don't store the keys in a key container
            publicKey = rsa.ExportParameters(false);
            privateKey = rsa.ExportParameters(true);
        }
    }
    /// <summary>
    /// Encrypts the given byte array with the RSA standard
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public byte[] Encrypt(string message)
    {
        byte[] input = Encoding.UTF8.GetBytes(message);
        byte[] encrypted;
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            rsa.PersistKeyInCsp = false;
            rsa.ImportParameters(publicKey);
            encrypted = rsa.Encrypt(input, RSAEncryptionPadding.Pkcs1);
        }
        return encrypted;
    }
    /// <summary>
    /// Decrypts the given byte array with the RSA standard
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string Decrypt(byte[] encrypted)
    {
        byte[] decrypted;
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            rsa.PersistKeyInCsp = false;
            rsa.ImportParameters(privateKey);
            decrypted = rsa.Decrypt(encrypted, RSAEncryptionPadding.Pkcs1);
        }
        return Encoding.UTF8.GetString(decrypted);
    }

    public void SetPublicKey(string publicKeyStr)
    {
        publicKey = JsonConvert.DeserializeObject<RSAParameters>(publicKeyStr); // Convert the JSON-Object into the rsa publicKey
    }

    #endregion
}

