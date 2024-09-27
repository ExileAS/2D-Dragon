using System;
using System.IO;
using System.Security.Cryptography;

public static class AESUtil
{
    public static string Encrypt(string plainText)
    {
        byte[] keyBytes = Convert.FromBase64String(GenKey());
        byte[] ivBytes;
        byte[] encrypted;

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.GenerateIV();
            ivBytes = aes.IV;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    encrypted = ms.ToArray();
                }
            }
        }
        byte[] combinedData = new byte[ivBytes.Length + encrypted.Length];
        Buffer.BlockCopy(ivBytes, 0, combinedData, 0, ivBytes.Length);
        Buffer.BlockCopy(encrypted, 0, combinedData, ivBytes.Length, encrypted.Length);
        return Convert.ToBase64String(combinedData);
    }


    public static string Decrypt(string cipherText)
    {
        byte[] keyBytes = Convert.FromBase64String(GenKey());
        byte[] combinedData = Convert.FromBase64String(cipherText);
        byte[] ivBytes = new byte[16];
        byte[] cipherBytes = new byte[combinedData.Length - 16];
        
        Buffer.BlockCopy(combinedData, 0, ivBytes, 0, ivBytes.Length);
        Buffer.BlockCopy(combinedData, ivBytes.Length, cipherBytes, 0, cipherBytes.Length);

        string plaintext = null;
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(cipherBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        plaintext = sr.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }

    private static string GenKey() {
        string key1 = "YMEgMAgFUzerPs4pA=";
        string key2 = "10E7ZMhH";
        string key3 = "Lk7W9Cni+vqz";
        string key4 = "14EPPT";
        return key4 + key2 + key3 + key1;
    }
}
