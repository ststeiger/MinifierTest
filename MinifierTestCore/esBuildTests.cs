
namespace MinifierTestCore;

using esBuildMinimizer;


internal class esBuildTests
{


    internal static async System.Threading.Tasks.Task Test()
    {
        // await RedditImageDownloader.Test();

        // https://github.com/microsoft/ajaxmin

        //Microsoft.Ajax.Utilities.Minifier msMinifier =
        //    new Microsoft.Ajax.Utilities.Minifier();

        //// Create the settings object and
        //// disable the stripping of debug statements
        //Microsoft.Ajax.Utilities.CodeSettings settings =
        //    new Microsoft.Ajax.Utilities.CodeSettings()
        //    {
        //        StripDebugStatements = false
        //    };

        string originalFileContent = System.IO.File.ReadAllText(@"D:\username\Documents\Visual Studio 2022\TFS\COR-CAFM-V4\CAFM\Portal_Visualiser\0\VWS.Plugin.Export.PDF.js");

        // string msContent = msMinifier.MinifyJavaScript(originalFileContent, settings);
        // System.Console.WriteLine(msContent);




        string esBuildDirectory = @"D:\username\Documents\Visual Studio 2022\gitlab\MinifierTest\esBuildMinimizer\esBuild\runtimes\windows\amd64\";
        string esContent2 = await EsBuildPipeRunner.MinifyWithSystemBinaryAsync(originalFileContent, SourceMapMode.None, esBuildDirectory);
        System.Console.WriteLine(esContent2);

        ESBuildResult result2 = await ESBuildFileRunner.MinifyWithSystemBinaryAsync(originalFileContent, SourceMapMode.Both, esBuildDirectory);
        System.Console.WriteLine(result2.JavaScript);
        System.Console.WriteLine(result2.SourceMap);


        string esContent = await EsBuildPipeRunner.MinifyWithEmbeddedBinaryAsync(originalFileContent, SourceMapMode.Inline);
        System.Console.WriteLine(esContent);

        ESBuildResult result = await ESBuildFileRunner.MinifyWithEmbeddedBinaryAsync(originalFileContent, SourceMapMode.Inline);
        System.Console.WriteLine(result.JavaScript);
        System.Console.WriteLine(result.SourceMap);
    } // End Task Test  


} // End Class esBuildTests 
