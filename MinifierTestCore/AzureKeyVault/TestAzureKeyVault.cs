
namespace MinifierTestCore
{


    internal static partial class TestAzureKeyVault
    {


        public static async System.Threading.Tasks.Task TestGet()
        {
            Azure.Security.KeyVault.Secrets.SecretClient client = 
                GetSecretClient();

            // The name and value for your new secret
            string secretName = "MyNewConnectionSecret";

            try
            {
                // Retrieve the secret
                Azure.Security.KeyVault.Secrets.KeyVaultSecret secret =
                    await client.GetSecretAsync(secretName);

                System.Console.WriteLine($"Your secret value is: {secret.Value}");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error creating secret: {ex.Message}");
            }
        } // End Task TestGet 


        public static async System.Threading.Tasks.Task TestSet()
        {
            // Use DefaultAzureCredential for easy authentication
            Azure.Security.KeyVault.Secrets.SecretClient client =
                GetSecretClient();

            // The name and value for your new secret
            string secretName = "MyNewConnectionSecret";
            string secretValue = "Server=tcp:myserver.database.windows.net;Initial Catalog=mydb;...";

            try
            {
                // This adds the secret to the vault. 
                // If the name already exists, it creates a new version.
                Azure.Security.KeyVault.Secrets.KeyVaultSecret result = 
                    await client.SetSecretAsync(secretName, secretValue);

                System.Console.WriteLine($"Secret '{result.Name}' created successfully.");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error creating secret: {ex.Message}");
            }
            
        } // End Task TestSet 


        public static async System.Threading.Tasks.Task 
            TestKeyOperations()
        {
            Azure.Security.KeyVault.Keys.KeyClient keyClient = 
                GetKeyClient();
            string keyName = "MyCryptoKey";

            // 1. Create a Key (RSA 2048)
            Azure.Security.KeyVault.Keys.KeyVaultKey key = 
                await keyClient.CreateRsaKeyAsync(
                    new Azure.Security.KeyVault.Keys
                    .CreateRsaKeyOptions(keyName)
            );

            // 2. Initialize CryptographyClient to use the key
            // We use the ID of the key we just created
            Azure.Security.KeyVault.Keys.Cryptography.CryptographyClient cryptoClient =
                GetCryptoClient(key.Id);

            // 3. Encrypt data
            string plainText = "Hello, this is sensitive!";
            byte[] dataToEncrypt = System.Text.Encoding.UTF8.GetBytes(plainText);

            Azure.Security.KeyVault.Keys.Cryptography.EncryptResult encryptResult = 
                await cryptoClient.EncryptAsync(
                Azure.Security.KeyVault.Keys.Cryptography.EncryptionAlgorithm.RsaOaep,
                dataToEncrypt
            );

            System.Console.WriteLine($"Encrypted: {System.Convert.ToBase64String(encryptResult.Ciphertext)}");

            // 4. Decrypt data
            Azure.Security.KeyVault.Keys.Cryptography.DecryptResult decryptResult = 
                await cryptoClient.DecryptAsync(
                Azure.Security.KeyVault.Keys.Cryptography.EncryptionAlgorithm.RsaOaep,
                encryptResult.Ciphertext
            );

            string decryptedText = System.Text.Encoding.UTF8.GetString(decryptResult.Plaintext);
            System.Console.WriteLine($"Decrypted: {decryptedText}");
        } // End Task TestKeyOperations 


    } // End Class TestAzureKeyVault 


} // End Namespace 
