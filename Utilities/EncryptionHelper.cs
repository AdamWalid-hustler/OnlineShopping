using System.Security.Cryptography;
using System.Text;

namespace OnlineShopping.Utilities
{
    public static class EncryptionHelper
    {
        
     
        private static readonly string EncryptionKey = "UnbeatableEncription!"; 
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("MyFirstSalt123"); 

        // Encrypt a string
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                // Convert plain text to bytes
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                
                // Derive key from password
                using (var deriveBytes = new Rfc2898DeriveBytes(EncryptionKey, Salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] keyBytes = deriveBytes.GetBytes(32); // 256-bit key
                    
                    
                    byte[] encrypted = new byte[plainBytes.Length];
                    for (int i = 0; i < plainBytes.Length; i++)
                    {
                        encrypted[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
                    }
                    
                    return Convert.ToBase64String(encrypted);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encryption error: {ex.Message}");
                return plainText;
            }
        }

        // Decrypt a string
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                
                byte[] encrypted = Convert.FromBase64String(cipherText);
                
                // Same key from password
                using (var deriveBytes = new Rfc2898DeriveBytes(EncryptionKey, Salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] keyBytes = deriveBytes.GetBytes(32); // 256-bit key
                    
                    // XOR decryption 
                    byte[] decrypted = new byte[encrypted.Length];
                    for (int i = 0; i < encrypted.Length; i++)
                    {
                        decrypted[i] = (byte)(encrypted[i] ^ keyBytes[i % keyBytes.Length]);
                    }
                    
                    return Encoding.UTF8.GetString(decrypted);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption error: {ex.Message}");
                return cipherText; 
            }
        }
    }
}
