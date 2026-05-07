
namespace MinifierTestCore;

internal class Program
{


    internal static async System.Threading.Tasks.Task<int> 
        Main(string[] args)
    {
        // await TestAzureKeyVault.TestUnimplemented();

        // await TestAzureKeyVault.TestSet();
        // await TestAzureKeyVault.TestGet();

        // await TestAzureKeyVault.TestKeyOperations();
        await esBuildTests.Test();

        await System.Console.Out.WriteLineAsync(" --- Press any key to continue --- ");
        return 0;
    } // End Task Main 


} // End Class Program 
