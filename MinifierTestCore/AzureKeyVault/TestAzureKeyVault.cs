
namespace MinifierTestCore
{


    internal static partial class TestAzureKeyVault
    {


        public static async System.Threading.Tasks.Task TestListSecrets()
        {
            Azure.Security.KeyVault.Secrets.SecretClient client = GetSecretClient();

            try
            {
                // GetPropertiesOfSecretsAsync returns an AsyncPageable of SecretProperties
                // It handles pagination automatically under the hood
                Azure.AsyncPageable<Azure.Security.KeyVault.Secrets.SecretProperties> secretProperties = client.GetPropertiesOfSecretsAsync();

                await foreach (Azure.Security.KeyVault.Secrets.SecretProperties secret in secretProperties)
                {
                    System.Console.WriteLine($"Found secret: {secret.Name}");

                    // If you need the value, you must call GetSecretAsync for each one
                    // Azure.Security.KeyVault.Secrets.KeyVaultSecret fullSecret = 
                    //    await client.GetSecretAsync(secret.Name);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error listing secrets: {ex.Message}");
            }
        }

        public static async System.Threading.Tasks.Task TestListSecretVersions(
            string secretName
        )
        {
            Azure.Security.KeyVault.Secrets.SecretClient client = GetSecretClient();

            try
            {
                // GetPropertiesOfSecretVersionsAsync returns all versions of the specified secret
                Azure.AsyncPageable<Azure.Security.KeyVault.Secrets.SecretProperties> versions =
                    client.GetPropertiesOfSecretVersionsAsync(secretName);

                await foreach (Azure.Security.KeyVault.Secrets.SecretProperties version in versions)
                {
                    System.Console.WriteLine($"Found version: {version.Version}, Created: {version.CreatedOn}");

                    // If you need to access a specific version's value, use:
                    // KeyVaultSecret secretVersion = await client.GetSecretAsync(secretName, version.Version);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error listing versions for {secretName}: {ex.Message}");
            }
        }

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
                System.Console.WriteLine($"Error getting secret: {ex.Message}");
            }
        } // End Task TestGet 


        public static async System.Threading.Tasks.Task DeleteSecret(string secretName)
        {
            try
            {
                Azure.Security.KeyVault.Secrets.SecretClient client = GetSecretClient();

                // Initiates the deletion process
                Azure.Security.KeyVault.Secrets.DeleteSecretOperation operation =
                    await client.StartDeleteSecretAsync(secretName);

                // Optional: Wait for the deletion to complete (polling)
                await operation.WaitForCompletionAsync();

                System.Console.WriteLine($"Secret '{secretName}' has been deleted.");
            }
            catch (System.Exception ex)
            {
                await System.Console.Error.WriteLineAsync("Error deleting secret: \r\n" + ex.Message);
            }
        }

        public static async System.Threading.Tasks.Task DeleteKey(string keyName)
        {
            try
            {
                Azure.Security.KeyVault.Keys.KeyClient client = GetKeyClient();

                // Initiates the deletion process
                Azure.Security.KeyVault.Keys.DeleteKeyOperation operation =
                    await client.StartDeleteKeyAsync(keyName);

                // Optional: Wait for the deletion to complete (polling)
                await operation.WaitForCompletionAsync();

                System.Console.WriteLine($"Key '{keyName}' has been deleted.");
            }
            catch (System.Exception ex)
            {
                await System.Console.Error.WriteLineAsync("Error deleting key: \r\n" + ex.Message);
            }

        }


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

        public static async System.Threading.Tasks.Task TestListKeyAsync(
            System.Threading.CancellationToken ct = default
        )
        {
            Azure.Security.KeyVault.Keys.KeyClient keyVaultClient = GetKeyClient();

            Azure.AsyncPageable<Azure.Security.KeyVault.Keys.KeyProperties> rows = 
                keyVaultClient.GetPropertiesOfKeysAsync(ct);

            await foreach (Azure.Security.KeyVault.Keys.KeyProperties row in rows)
            {
                System.Console.WriteLine($"Name: {row.Name}, CreatedOn; {row.CreatedOn}, Enabled: {row.Enabled}");
            }

        }

        public static async System.Threading.Tasks.Task TestListKeyVersionsAsync(
            string name,
            System.Threading.CancellationToken ct = default
        )
        {
            Azure.Security.KeyVault.Keys.KeyClient keyVaultClient = GetKeyClient();

            // Fetch raw versions from SDK
            Azure.AsyncPageable<Azure.Security.KeyVault.Keys.KeyProperties> rows = keyVaultClient.GetPropertiesOfKeyVersionsAsync(name, ct);

            await foreach (Azure.Security.KeyVault.Keys.KeyProperties row in rows)
            {
                System.Console.WriteLine($"Name: {row.Name}, Version; {row.Version}, UpdatedOn: {row.UpdatedOn}");
            }

        }

    } // End Class TestAzureKeyVault 


} // End Namespace 
