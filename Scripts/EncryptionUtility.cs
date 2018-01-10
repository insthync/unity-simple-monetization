using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class EncryptionUtility
{
    public static string GenerateIV()
    {
        return Guid.NewGuid().ToString().Substring(0, 16);
    }

    public static string AESEncrypt(string Text, string Key, string IV)
    {
        byte[] byteKey = Encoding.UTF8.GetBytes(Key);
        byte[] byteIV = Encoding.UTF8.GetBytes(IV);
        return AESEncrypt(Text, byteKey, byteIV);
    }

    public static string AESEncrypt(string Text, byte[] Key, byte[] IV)
    {
        string result = "";

        // Check arguments. 
        if (Text == null || Text.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        #region AesManaged
        byte[] encrypted;
        // Create an AesManaged object 
        // with the specified key and IV. 
        using (AesManaged aesAlg = new AesManaged())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            aesAlg.BlockSize = 128;
            aesAlg.Padding = PaddingMode.Zeros;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption. 
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(Text);
                    }
                    encrypted = msEncrypt.ToArray();
                    result = Convert.ToBase64String(encrypted);
                }
            }
        }
        #endregion
        return result;
    }

    public static string AESDecrypt(string Text, string Key, string IV)
    {
        byte[] byteKey = Encoding.UTF8.GetBytes(Key);
        byte[] byteIV = Encoding.UTF8.GetBytes(IV);
        return AESDecrypt(Text, byteKey, byteIV);
    }

    public static string AESDecrypt(string Text, byte[] Key, byte[] IV)
    {
        string result = "";

        // Check arguments. 
        if (Text == null || Text.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        #region AesManaged
        // Create an AesManaged object 
        // with the specified key and IV. 
        using (AesManaged aesAlg = new AesManaged())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            aesAlg.BlockSize = 128;
            aesAlg.Padding = PaddingMode.Zeros;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption. 
            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(Text)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream 
                        // and place them in a string.
                        result = srDecrypt.ReadToEnd();
                    }
                }
            }

        }
        #endregion
        return result;
    }

    public static string MD5Encrypt(string str)
    {
        byte[] input = Encoding.UTF8.GetBytes(str);
        return MD5Encrypt(input);
    }

    public static string MD5Encrypt(byte[] input)
    {
        // need MD5 to calculate the hash
        byte[] digest = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(input);

        return BitConverter.ToString(digest).Replace("-", "").ToLower();
    }
}
