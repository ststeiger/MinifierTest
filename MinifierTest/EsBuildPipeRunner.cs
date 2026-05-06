
namespace MinifierTest
{


    public static class EsBuildPipeRunner
    {


        public static async System.Threading.Tasks.Task<string>
            MinifyWithEmbeddedBinaryAsync(
            string js,
            SourceMapMode sourceMapMode = SourceMapMode.None
        )
        {
            string output;

            if (
                sourceMapMode != SourceMapMode.None &&
                sourceMapMode != SourceMapMode.Inline
            )
            {
                throw new System.NotSupportedException(
                    "Pipe runner only supports SourceMapMode 'None' and 'Inline'."
                );
            }


            string tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "esbuild_" + System.Guid.NewGuid());

            try
            {
                if (!System.IO.Directory.Exists(tempDir))
                    System.IO.Directory.CreateDirectory(tempDir);

                string executable = ESBuildExecutables.ExtractEmbeddedEsbuild(tempDir);

                System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    
                    RedirectStandardInput = true,
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

                    // esbuild hat sogenannte Loader,die sagen:
                    // Wie soll diese Eingabe interpretiert werden?
                    // js → JavaScript
                    // ts → TypeScript
                    // jsx → React JSX
                    // json → JSON
                    // css → CSS
                    // nur, wenn esbuild nicht automatisch erkennt

                    // Arguments = "--minify --sourcemap=inline --loader=js",

                    Arguments =
                        $"--minify " +
                        (
                            sourceMapMode == SourceMapMode.None ?
                             "": "--sourcemap=inline "
                        ) +
                        $"--loader=js ",

                }
            ;

                using (System.Diagnostics.Process process =
                    System.Diagnostics.Process.Start(psi)
                )
                {
                    // await process.StandardInput.WriteAsync(js);
                    // process.StandardInput.Close();

                    // fix: input utf8 on .NET 4.8
                    using (System.IO.StreamWriter writer =
                        new System.IO.StreamWriter(
                            process.StandardInput.BaseStream,
                            new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                            bufferSize: 1024,
                            leaveOpen: true
                        )
                    )
                    {
                        await writer.WriteAsync(js);
                        await writer.FlushAsync();
                    } // End Using writer 

                    // not necessary, close flushes anyway 
                    // await process.StandardInput.FlushAsync();
                    process.StandardInput.Close();

                    // System.Threading.Tasks.Task processExit = process.WaitForExitAsync();
                    System.Threading.Tasks.Task processExit = PipeRunnerHelper.WaitForExitAsync(process);

                    System.Threading.Tasks.Task<string> stdOutTask = process.StandardOutput.ReadToEndAsync();
                    System.Threading.Tasks.Task<string> stdErrTask = process.StandardError.ReadToEndAsync();

                    await processExit;

                    output = await stdOutTask;
                    string error = await stdErrTask;

                    if (process.ExitCode != 0)
                        throw new System.Exception(error);
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

            return output;
        } // End Task MinifyWithEmbeddedBinaryAsync 


        public static async System.Threading.Tasks.Task<string>
            MinifyWithSystemBinaryAsync(
            string js,
            SourceMapMode sourceMapMode = SourceMapMode.None,
            string executableDir = null
        )
        {
            string output;


            if (
                sourceMapMode != SourceMapMode.None &&
                sourceMapMode != SourceMapMode.Inline
            )
            {
                throw new System.NotSupportedException(
                    "Pipe runner only supports SourceMapMode 'None' and 'Inline'."
                );
            }

            if (!string.IsNullOrEmpty(executableDir))
            {
                if (!System.IO.Directory.Exists(executableDir))
                    throw new System.IO.DirectoryNotFoundException(executableDir);

                PipeRunnerHelper.AddToCurrentProcessPath(executableDir);
            } // End if (!string.IsNullOrEmpty(executableDir))

            System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,

                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,

                    // That API only exists in modern .NET (Core / 5+).
                    // StandardInputEncoding = System.Text.Encoding.UTF8, 
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8,
                    
                    FileName = "esbuild",

                    // --sourcemap            // external file (default = linked)
                    // --sourcemap=inline     // base64 inside JS
                    // --sourcemap=external   // separate .map file, no comment
                    // --sourcemap=linked     // separate file + //# sourceMappingURL
                    // --sourcemap=both       // inline + external (rare)

                    // esbuild hat sogenannte Loader,die sagen:
                    // Wie soll diese Eingabe interpretiert werden?
                    // js → JavaScript
                    // ts → TypeScript
                    // jsx → React JSX
                    // json → JSON
                    // css → CSS
                    // nur, wenn esbuild nicht automatisch erkennt

                    // Arguments = "--minify --sourcemap=inline --loader=js",
                    Arguments =
                        $"--minify " +
                        (
                            sourceMapMode == SourceMapMode.None ?
                             "" : "--sourcemap=inline "
                        ) +
                        $"--loader=js ",

                };


            using (System.Diagnostics.Process process =
                System.Diagnostics.Process.Start(psi)
            )
            {
                // await process.StandardInput.WriteAsync(js);
                // process.StandardInput.Close();

                // fix: input utf8 on .NET 4.8
                using (System.IO.StreamWriter writer =
                    new System.IO.StreamWriter(
                        process.StandardInput.BaseStream,
                        new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                        bufferSize: 1024,
                        leaveOpen: true
                    )
                )
                {
                    await writer.WriteAsync(js);
                    await writer.FlushAsync();
                } // End Using writer 

                // not necessary, close flushes anyway 
                // await process.StandardInput.FlushAsync();
                process.StandardInput.Close();

                // System.Threading.Tasks.Task processExit = process.WaitForExitAsync();
                System.Threading.Tasks.Task processExit = PipeRunnerHelper.WaitForExitAsync(process);

                System.Threading.Tasks.Task<string> stdOutTask = process.StandardOutput.ReadToEndAsync();
                System.Threading.Tasks.Task<string> stdErrTask = process.StandardError.ReadToEndAsync();

                await processExit;

                output = await stdOutTask;
                string error = await stdErrTask;

                if (process.ExitCode != 0)
                    throw new System.Exception(error);
            } // End Using process 

            return output;
        } // End Task MinifyWithSystemBinaryAsync 


    } // End Class EsBuildPipeRunner 


} // End Namespace 

