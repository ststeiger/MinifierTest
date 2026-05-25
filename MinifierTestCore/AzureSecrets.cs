
namespace MinifierTestCore;

// for use with azure-keyvault-emulator: 
// https://github.com/james-gould/azure-keyvault-emulator
internal class AzureSecrets
{
    // public static readonly string EmulatorToken = "TOP_SECRET";
    public static readonly string EmulatorToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE4OTAyMzkwMjIsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDEvIn0.bHLeGTRqjJrmIJbErE-1Azs724E5ibzvrIc-UQL6pws";
    // public static readonly string KeyVaultUri = "https://localhost:44395";
    public static readonly string KeyVaultUri = "https://localhost:54410";
}

// see also: 
// https://github.com/cricketthomas/AzureKeyVaultExplorer/blob/master/docs/FIRST-TIME-SETUP.md
// https://github.com/Azure/AzureKeyVault

// azure-cli:
// az keyvault secret set --vault-name https://localhost:4997 --name "TestSecret" --value "YourValue"

// https://jamesgould.dev/posts/Azure-Key-Vault-Emulator/
// https://github.com/Basis-Theory/azure-keyvault-emulator
