
using System.Linq;

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
        // await esBuildTests.Test();

        // await BundlerForAI.Test();


        string path = @"D:\inetpub";
        // var a = FileSystemScanner.EnumerateFilesRecursive(path);
        // var a = FileSystemScanner.EnumerateFilesQueue(path);
        // var a = FileSystemScanner.EnumerateFilesRecursiveOrdered(path);
        // var a = FileSystemScanner.EnumerateFilesLinkedList(path);
        // var a = FileSystemScanner.EnumerateFilesLinkedListOrder(path);
        // var a = FileSystemScanner.EnumerateFilesNoReverse(path);
        var a = FileSystemScanner.EnumerateFilesSafe(path);
        string list = string.Join(System.Environment.NewLine, a.ToList());
        System.Console.WriteLine(list);





    await System.Console.Out.WriteLineAsync("-- - Press any key to continue --- ");
        return 0;
    } // End Task Main 


} // End Class Program 
