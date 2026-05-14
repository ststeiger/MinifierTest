namespace MinifierTestCore;

internal class Program
{
    
    
    internal static async System.Threading.Tasks.Task<int> Main(string[] args)
    {
        // await TestAzureKeyVault.TestUnimplemented();

        // await TestAzureKeyVault.TestSet();
        // await TestAzureKeyVault.TestGet();

        // await TestAzureKeyVault.TestKeyOperations();
        // await esBuildTests.Test();

        await BundlerForAI.Test();


        string path = @"D:\inetpub";

        System.Collections.Generic.IEnumerable<string>? a = null;

        // a = FileSystemScanner.EnumerateFilesRecursive(path);
        // a = FileSystemScanner.EnumerateFilesQueueReorder(path);
        // a = FileSystemScanner.EnumerateFilesRecursiveOrdered(path);
        // a = FileSystemScanner.EnumerateFilesLinkedList(path);
        // a = FileSystemScanner.EnumerateFilesLinkedListOrder(path);
        // a = FileSystemScanner.EnumerateFilesNoReverse(path);
        a = FileSystemScanner.EnumerateFilesSafe(path);
        string list = string.Join(System.Environment.NewLine, System.Linq.Enumerable.ToList(a));
        System.Console.WriteLine(list);
        
        await System.Console.Out.WriteLineAsync("-- - Press any key to continue --- ");
        return 0;
    } // End Task Main
    
    
} // End Class Program 