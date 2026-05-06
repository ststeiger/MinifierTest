
namespace MinifierTest
{

    internal static class ESBuildExecutables
    {

        internal static string ExtractEmbeddedEsbuild(string targetDirectory)
        {
            string searchPattern;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                System.Runtime.InteropServices.Architecture arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;

                if (arch == System.Runtime.InteropServices.Architecture.Arm64)
                {
                    searchPattern = "windows.arm64.esbuild.exe";
                }
                else if (arch == System.Runtime.InteropServices.Architecture.X86)
                {
                    searchPattern = "windows.x86.esbuild.exe";
                }
                else if (arch == System.Runtime.InteropServices.Architecture.X64)
                {
                    searchPattern = "windows.amd64.esbuild.exe";
                }
                else
                {
                    throw new System.NotSupportedException(arch.ToString());
                }
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                searchPattern = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.Arm64
                    ? "linux.arm64.esbuild"
                    : "linux.amd64.esbuild";
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                searchPattern = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.Arm64
                    ? "darwin.arm64.esbuild"
                    : "darwin.amd64.esbuild";
            }
            else
            {
                throw new System.PlatformNotSupportedException("Unsupported operating system.");
            }

            System.Reflection.Assembly assembly = typeof(ESBuildExecutables).Assembly;
            string[] manifestResourceNames = assembly.GetManifestResourceNames();

            // Match resource names without needing the LogicalName property
            string resourceName = null;
            for (int i = 0; i < manifestResourceNames.Length; i++)
            {
                if (manifestResourceNames[i].EndsWith(searchPattern, System.StringComparison.OrdinalIgnoreCase))
                {
                    resourceName = manifestResourceNames[i];
                    break;
                }
            } // Next i 

            if (resourceName == null)
                throw new System.IO.FileNotFoundException($"Embedded resource matching '{searchPattern}' was not found.");

            bool isUnix = !System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            string destPath = System.IO.Path.Combine(targetDirectory, "esbuild" + (isUnix ? "" : ".exe"));

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new System.IO.FileNotFoundException($"Failed to load the resource stream '{resourceName}'.");

                using (System.IO.FileStream fileStream = new System.IO.FileStream(destPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                } // End Using fileStream

            } // End Using stream

            // Grant execution permissions on Linux/macOS
            if (isUnix)
                System.Diagnostics.Process.Start("chmod",
                    $"+x \"{destPath}\""
                ).WaitForExit();

            return destPath;
        } // End Task ExtractEmbeddedEsbuild 


    }// End Class ESBuildExecutables 


} // End Namespace 
