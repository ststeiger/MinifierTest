
namespace MinifierTestCore;

internal class Program
{
    
    
    internal static async System.Threading.Tasks.Task<int> Main(string[] args)
    {
        // await TestAzureKeyVault.TestUnimplemented();

        await TestAzureKeyVault.TestGet();
        await TestAzureKeyVault.TestSet();
        await TestAzureKeyVault.TestGet();

        await TestAzureKeyVault.TestListSecrets();
        await TestAzureKeyVault.TestListSecretVersions("MyNewConnectionSecret");
        await TestAzureKeyVault.DeleteSecret("MyNewConnectionSecret");
        await TestAzureKeyVault.DeleteKey("MyCryptoKey");


        await TestAzureKeyVault.TestKeyOperations();
        await TestAzureKeyVault.TestListKeyAsync();
        await TestAzureKeyVault.TestListKeyVersionsAsync("MyCryptoKey");

        // await esBuildTests.Test();

        // FileSystemScanner.Test();

        // await BundlerForAI.BundleDynamicSNI();
        // await BundlerForAI.BundleDbAdmin();

        // await BundlerForAI.BundleMobile2();
        // await BundlerForAI.Test();
        // await BundlerForAI.BundleKeyVaultEmulator();



        await System.Console.Out.WriteLineAsync("-- - Press any key to continue --- ");
        return 0;
    } // End Task Main
    
    
} // End Class Program 