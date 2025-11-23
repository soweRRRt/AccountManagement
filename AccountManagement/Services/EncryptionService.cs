using System;
using System.Security.Cryptography;
using System.Text;

namespace AccountManagement.Services;

public static class EncryptionService
{
    private static string _masterPassword = "YourSecureMasterPassword123!";

    public static void SetMasterPassword(string password)
    {
        _masterPassword = password;
    }

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);

        using (Aes encryptor = Aes.Create())
        {
            byte[] key = DeriveKeyFromPassword(_masterPassword);
            encryptor.Key = key;
            encryptor.GenerateIV();

            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(encryptor.IV, 0, encryptor.IV.Length);

                using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using (Aes encryptor = Aes.Create())
        {
            byte[] key = DeriveKeyFromPassword(_masterPassword);
            encryptor.Key = key;

            using (var ms = new System.IO.MemoryStream(cipherBytes))
            {
                byte[] iv = new byte[encryptor.IV.Length];
                ms.Read(iv, 0, iv.Length);
                encryptor.IV = iv;

                using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[cipherBytes.Length];
                    int decryptedByteCount = cs.Read(decryptedBytes, 0, decryptedBytes.Length);

                    return Encoding.Unicode.GetString(decryptedBytes, 0, decryptedByteCount);
                }
            }
        }
    }

    private static byte[] DeriveKeyFromPassword(string password)
    {
        byte[] salt = Encoding.ASCII.GetBytes("PasswordManagerSalt2024");

        using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            return deriveBytes.GetBytes(32); // 256 бит
        }
    }

    public static string GeneratePassword(int length = 16, bool includeSpecialChars = true)
    {
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";
        const string special = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        string chars = lowercase + uppercase + numbers;
        if (includeSpecialChars)
            chars += special;

        var random = new Random();
        var password = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            password.Append(chars[random.Next(chars.Length)]);
        }

        return password.ToString();
    }
}
