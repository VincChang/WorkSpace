using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace System;

/// <summary>
/// 字串加/解密之方法區, _salt 為主鍵值, 每次計算時, 都必須搭配各次計算時自己的 workingKey = sharedSecret
/// </summary>
public static class extCrypto
{
    private static byte[] _salt = Encoding.ASCII.GetBytes("holyYuantaCorp");

    /// <summary>
    /// Encrypt the given string using AES.  The string can be decrypted using 
    /// DecryptStringAES().  The sharedSecret parameters must match.
    /// </summary>
    /// <param name="plainText">The text to encrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
    public static string xEncryptString(this string plainText, string sharedSecret)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));
        if (string.IsNullOrEmpty(sharedSecret))
            throw new ArgumentNullException(nameof(sharedSecret));

        using (var aesAlg = Aes.Create())
        {
            // Derive the key
            var key = new Rfc2898DeriveBytes(sharedSecret, _salt, 10000, HashAlgorithmName.SHA256);
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

            // IV 是隨機產生的，將加密後與 IV 一起儲存
            aesAlg.GenerateIV();

            using (var msEncrypt = new MemoryStream())
            {
                // 寫入 IV 長度與 IV 本身
                msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (var csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    /// <summary>
    /// Decrypt the given string.  Assumes the string was encrypted using 
    /// EncryptStringAES(), using an identical sharedSecret.
    /// </summary>
    /// <param name="cipherText">The text to decrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
    public static string xDecryptString(this string cipherText, string sharedSecret)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException(nameof(cipherText));
        if (string.IsNullOrEmpty(sharedSecret))
            throw new ArgumentNullException(nameof(sharedSecret));

        byte[] fullCipher = Convert.FromBase64String(cipherText);

        using (var msDecrypt = new MemoryStream(fullCipher))
        {
            // 讀取 IV 長度與 IV 值
            byte[] rawLength = new byte[sizeof(int)];
            if (msDecrypt.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
                throw new CryptographicException("Cannot read IV length");

            int ivLength = BitConverter.ToInt32(rawLength, 0);
            byte[] iv = new byte[ivLength];
            if (msDecrypt.Read(iv, 0, iv.Length) != iv.Length)
                throw new CryptographicException("Cannot read IV value");

            using (var aesAlg = Aes.Create())
            {
                var key = new Rfc2898DeriveBytes(sharedSecret, _salt, 10000, HashAlgorithmName.SHA256);
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = iv;

                using (var csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}