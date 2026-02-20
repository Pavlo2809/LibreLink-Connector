using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace LibreLinkConnector.Services
{
    public class CredentialManager
    {
        private static readonly string CredentialsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LibreLinkConnector",
            "credentials.dat"
        );

        public class StoredCredentials
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string? AuthToken { get; set; }
            public long? TokenExpiry { get; set; }
            public bool RememberMe { get; set; }
        }

        public static void SaveCredentials(StoredCredentials credentials)
        {
            try
            {
                var directory = Path.GetDirectoryName(CredentialsPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(credentials);
                var bytes = Encoding.UTF8.GetBytes(json);

                // Encrypt data using Windows Data Protection API
                var encryptedBytes = ProtectedData.Protect(
                    bytes,
                    null,
                    DataProtectionScope.CurrentUser
                );

                File.WriteAllBytes(CredentialsPath, encryptedBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save credentials: {ex.Message}", ex);
            }
        }

        public static StoredCredentials? LoadCredentials()
        {
            try
            {
                if (!File.Exists(CredentialsPath))
                {
                    return null;
                }

                var encryptedBytes = File.ReadAllBytes(CredentialsPath);

                // Decrypt data using Windows Data Protection API
                var bytes = ProtectedData.Unprotect(
                    encryptedBytes,
                    null,
                    DataProtectionScope.CurrentUser
                );

                var json = Encoding.UTF8.GetString(bytes);
                return JsonConvert.DeserializeObject<StoredCredentials>(json);
            }
            catch (Exception)
            {
                // If decryption fails or file is corrupted, return null
                return null;
            }
        }

        public static void DeleteCredentials()
        {
            try
            {
                if (File.Exists(CredentialsPath))
                {
                    File.Delete(CredentialsPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete credentials: {ex.Message}", ex);
            }
        }

        public static bool HasStoredCredentials()
        {
            return File.Exists(CredentialsPath);
        }
    }
}
