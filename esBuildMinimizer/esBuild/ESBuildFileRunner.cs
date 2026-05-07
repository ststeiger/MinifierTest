
namespace esBuildMinimizer
{


    public static class ESBuildFileRunner
    {


        public static async System.Threading.Tasks.Task<ESBuildResult>
            MinifyWithEmbeddedBinaryAsync(
            string jsInput,
            SourceMapMode sourceMapMode = SourceMapMode.None
        )
        {
            ESBuildResult result;
            string tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "esbuild_" + System.Guid.NewGuid());

            try
            {
                if (!System.IO.Directory.Exists(tempDir))
                    System.IO.Directory.CreateDirectory(tempDir);

                string executable = ESBuildExecutables.ExtractEmbeddedEsbuild(tempDir);

                string inputFile = System.IO.Path.Combine(tempDir, "input.js");
                string outputFile = System.IO.Path.Combine(tempDir, "output.js");

                await PipeRunnerHelper.WriteAllTextAsync(inputFile, jsInput, System.Text.Encoding.UTF8);

                System.Diagnostics.ProcessStartInfo psi =
                    new System.Diagnostics.ProcessStartInfo()
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,

                        RedirectStandardOutput = true,
                        RedirectStandardError = true,

                        // That API only exists in modern .NET (Core / 5+).
                        // StandardInputEncoding = System.Text.Encoding.UTF8, 
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                        StandardErrorEncoding = System.Text.Encoding.UTF8,
                        
                        FileName = executable,

                        // --sourcemap            // external file (default = linked)
                        // --sourcemap=inline     // base64 inside JS
                        // --sourcemap=external   // separate .map file, no comment
                        // --sourcemap=linked     // separate file + //# sourceMappingURL
                        // --sourcemap=both       // inline + external (rare)

                        Arguments =
                        $"\"{inputFile}\" " +
                        $"--minify " +
                         (
                            sourceMapMode == SourceMapMode.None ?
                             "" : $"--sourcemap={sourceMapMode.ToString().ToLowerInvariant()} "
                        ) +
                        $"--outfile=\"{outputFile}\" ",

                    };

                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi))
                {
                    // System.Threading.Tasks.Task processExit = process.WaitForExitAsync();
                    System.Threading.Tasks.Task processExit = PipeRunnerHelper.WaitForExitAsync(process);

                    System.Threading.Tasks.Task<string> stdOutTask = process.StandardOutput.ReadToEndAsync();
                    System.Threading.Tasks.Task<string> stdErrTask = process.StandardError.ReadToEndAsync();

                    await processExit;

                    string stdout = await stdOutTask;
                    string error = await stdErrTask;

                    if (process.ExitCode != 0)
                        throw new System.Exception(error);

                    if (!System.IO.File.Exists(outputFile))
                        throw new System.Exception("JavaScript file was not generated.");

                    string resultJs = await PipeRunnerHelper.ReadAllTextAsync(outputFile);

                    string resultMap = null;
                    if (sourceMapMode != SourceMapMode.None 
                        && sourceMapMode != SourceMapMode.Inline
                    )
                    {
                        string mapFile = outputFile + ".map";
                        if (!System.IO.File.Exists(mapFile))
                            throw new System.Exception("Source map was not generated.");

                        resultMap = await PipeRunnerHelper.ReadAllTextAsync(mapFile);
                    } // End if (sourceMapMode != SourceMapMode.None | Inline) 

                    result = new ESBuildResult(resultJs, resultMap);
                } // End Using process 
            } // End Try 
            finally
            {
                try
                {
                    System.IO.Directory.Delete(tempDir, recursive: true);
                } // End Try 
                catch (System.Exception ex)
                {
                    // ignore cleanup errors (locked files, AV, etc.)
                    await System.Console.Error.WriteLineAsync(ex.Message);
                } // End Catch 
            } // End Finally 

            return result;
        } // End Task MinifyWithEmbeddedBinaryAsync 


        public static async System.Threading.Tasks.Task<ESBuildResult>
            MinifyWithSystemBinaryAsync(
            string js,
            SourceMapMode sourceMapMode = SourceMapMode.None,
            string executableDir = null
        )
        {
            ESBuildResult result;
            string tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "esbuild_" + System.Guid.NewGuid());

            try
            {
                if (!string.IsNullOrEmpty(executableDir))
                {
                    if (!System.IO.Directory.Exists(executableDir))
                        throw new System.IO.DirectoryNotFoundException(executableDir);

                    PipeRunnerHelper.AddToCurrentProcessPath(executableDir);
                } // End if (!string.IsNullOrEmpty(executableDir))

                if (!System.IO.Directory.Exists(tempDir))
                    System.IO.Directory.CreateDirectory(tempDir);

                string inputFile = System.IO.Path.Combine(tempDir, "input.js");
                string outputFile = System.IO.Path.Combine(tempDir, "output.js");

                await PipeRunnerHelper.WriteAllTextAsync(inputFile, js, System.Text.Encoding.UTF8);

                System.Diagnostics.ProcessStartInfo psi =
                    new System.Diagnostics.ProcessStartInfo()
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,

                        RedirectStandardOutput = true,
                        RedirectStandardError = true,

                        // That API only exists in modern .NET (Core / 5+).
                        // StandardInputEncoding = System.Text.Encoding.UTF8, 
                        StandardErrorEncoding = System.Text.Encoding.UTF8,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,

                        FileName = "esbuild",

                        // --sourcemap            // external file (default = linked)
                        // --sourcemap=inline     // base64 inside JS
                        // --sourcemap=external   // separate .map file, no comment
                        // --sourcemap=linked     // separate file + //# sourceMappingURL
                        // --sourcemap=both       // inline + external (rare)

                        Arguments =
                        $"\"{inputFile}\" " +
                        $"--minify " +
                         (
                            sourceMapMode == SourceMapMode.None ?
                             "" : $"--sourcemap={sourceMapMode.ToString().ToLowerInvariant()} "
                        ) +
                        $"--outfile=\"{outputFile}\" ",
                    };

                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi))
                {
                    // System.Threading.Tasks.Task processExit = process.WaitForExitAsync();
                    System.Threading.Tasks.Task processExit = PipeRunnerHelper.WaitForExitAsync(process);

                    System.Threading.Tasks.Task<string> stdOutTask = process.StandardOutput.ReadToEndAsync();
                    System.Threading.Tasks.Task<string> stdErrTask = process.StandardError.ReadToEndAsync();

                    await processExit;

                    string stdout = await stdOutTask;
                    string error = await stdErrTask;

                    if (process.ExitCode != 0)
                        throw new System.Exception(error);

                    if (!System.IO.File.Exists(outputFile))
                        throw new System.Exception("JavaScript file was not generated.");

                    string resultJs = await PipeRunnerHelper.ReadAllTextAsync(outputFile);
                    string resultMap = null;

                    if (sourceMapMode != SourceMapMode.None
                        && sourceMapMode != SourceMapMode.Inline
                    )
                    {
                        string mapFile = outputFile + ".map";
                        if (!System.IO.File.Exists(mapFile))
                            throw new System.Exception("Source map was not generated.");

                        resultMap = await PipeRunnerHelper.ReadAllTextAsync(mapFile);
                    } // End if (sourceMapMode != SourceMapMode.None | Inline) 

                    result = new ESBuildResult(resultJs, resultMap);
                } // End Using process 
            } // End Try 
            finally
            {
                try
                {
                    System.IO.Directory.Delete(tempDir, recursive: true);
                } // End Try 
                catch (System.Exception ex)
                {
                    // ignore cleanup errors (locked files, AV, etc.)
                    await System.Console.Error.WriteLineAsync(ex.Message);
                } // End Catch 
            } // End Finally 

            return result;
        } // End Task MinifyWithSystemBinaryAsync 


    } // End Class ESBuildFileRunner 


} // End Namespace 
