
namespace MinifierTestCore;

// for use with azure-keyvault-emulator: 
// https://github.com/james-gould/azure-keyvault-emulator
internal class AzureSecrets
{
    public static readonly string EmulatorToken = "TOP_SECRET";
    public static readonly string KeyVaultUri = "https://localhost:44395";
}
// see also: 
// https://github.com/cricketthomas/AzureKeyVaultExplorer/blob/master/docs/FIRST-TIME-SETUP.md
// https://github.com/Azure/AzureKeyVault

// azure-cli:
// az keyvault secret set --vault-name https://localhost:4997 --name "TestSecret" --value "YourValue"

// https://jamesgould.dev/posts/Azure-Key-Vault-Emulator/
// https://github.com/Basis-Theory/azure-keyvault-emulator
