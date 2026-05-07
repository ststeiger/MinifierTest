
namespace MinifierTestCore
{

    // Methods not implemented by Server
    internal static partial class TestAzureKeyVault
    {
        public static async System.Threading.Tasks.Task TestUnimplemented()
        {
            await ListAllSecrets();
            await ListSecretsWithValues();

            await ListAllKeys();
        }

        private static async System.Threading.Tasks.Task ListAllKeys()
        {
            Azure.Security.KeyVault.Keys.KeyClient keyClient = GetKeyClient();

            System.Console.WriteLine("Listing key names:");

            await foreach (Azure.Security.KeyVault.Keys.KeyProperties keyProperties in keyClient.GetPropertiesOfKeysAsync())
            {
                System.Console.WriteLine($"- Key Name: {keyProperties.Name}");
            }
        } // End Task ListAllKeys 

        private static async System.Threading.Tasks.Task ListAllSecrets()
        {
            Azure.Security.KeyVault.Secrets.SecretClient secretClient = GetSecretClient();

            System.Console.WriteLine("Listing secret names:");

            // This gets only the metadata (names, expiration, etc.)
            await foreach (Azure.Security.KeyVault.Secrets.SecretProperties secretProperties in secretClient.GetPropertiesOfSecretsAsync())
            {
               System. Console.WriteLine($"- Found: {secretProperties.Name}");
            }
        } // End Task ListAllSecrets 

        private static async System.Threading.Tasks.Task ListSecretsWithValues()
        {
            Azure.Security.KeyVault.Secrets.SecretClient client = GetSecretClient();

            await foreach (Azure.Security.KeyVault.Secrets.SecretProperties secretProperties in client.GetPropertiesOfSecretsAsync())
            {
                // Now we fetch the actual sensitive value for each name
                Azure.Security.KeyVault.Secrets.KeyVaultSecret secret = await client.GetSecretAsync(secretProperties.Name);
                System.Console.WriteLine($"{secret.Name} = {secret.Value}");
            }
        } // End Task ListSecretsWithValues 


    } // End Class TestAzureKeyVault 


} // End Namespace 
