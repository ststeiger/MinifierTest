
namespace MinifierTestCore;

// dotnet add package VaultSharp
internal class TestHashicorpsVault 
{

    internal static VaultSharp.IVaultClient CreateClient()
    {
        // 1. Initialize Vault Client
        VaultSharp.V1.AuthMethods.Token.TokenAuthMethodInfo authMethod = new VaultSharp.V1.AuthMethods.Token.TokenAuthMethodInfo("your-vault-token");
        VaultSharp.VaultClientSettings vaultClientSettings = new VaultSharp.VaultClientSettings("https://your-vault-url:8200", authMethod);
        VaultSharp.IVaultClient vaultClient = new VaultSharp.VaultClient(vaultClientSettings);
        return vaultClient;
    }


    // package AWSSDK.SecretsManager 
    internal static async System.Threading.Tasks.Task CreateSecret()
    {
        // 1. Initialize Vault Client
        VaultSharp.IVaultClient vaultClient = CreateClient();
        
        // 2. Prepare Connection String Data
        System.Collections.Generic.Dictionary<string, object> secretData = 
            new System.Collections.Generic.Dictionary<string, object>
        {
            { "ConnectionString", "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;" }
        };
        
        // 3. Persist to Vault (KV Engine v2)
        await vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(
            path: "database-config",
            data: secretData,
            mountPoint: "secret" // The path where your KV engine is enabled
        );
    }
}
