
namespace MinifierTest
{


    internal static class PipeRunnerHelper 
    {


        internal static async System.Threading.Tasks.Task<string>
            ReadAllTextAsync(
            string path,
            System.Text.Encoding encoding = null
        )
        {
            encoding = encoding ?? System.Text.Encoding.UTF8;

            // FileMode.Open opens the existing file, and FileShare.Read allows concurrent reading
            using (System.IO.FileStream stream =
                new System.IO.FileStream(
                    path,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read,
                    System.IO.FileShare.Read,
                    4096,
                    System.IO.FileOptions.Asynchronous
                )
            )
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, encoding))
                {
                    return await reader.ReadToEndAsync();
                } // End using reader 

            } // End using stream 

        } // End Task ReadAllTextAsync 


        internal static async System.Threading.Tasks.Task
            WriteAllTextAsync(
            string path,
            string content,
            System.Text.Encoding encoding = null
        )
        {
            encoding = encoding ?? System.Text.Encoding.UTF8;

            // FileMode.Create overwrites the file if it exists, or creates a new one
            using (System.IO.FileStream stream =
                new System.IO.FileStream(
                    path,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.Write,
                    System.IO.FileShare.None,
                    4096,
                    System.IO.FileOptions.Asynchronous
                )
            )
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream, encoding))
                {
                    await writer.WriteAsync(content);
                } // End Using writer 

            } // End Using stream 

        } // End Task WriteAllTextAsync 


        internal static void AddToCurrentProcessPath(string executableDir)
        {
            // 1. Get the current PATH variable
            string currentPath = System.Environment.GetEnvironmentVariable("PATH", System.EnvironmentVariableTarget.Process);

            // If empty, just set it
            if (string.IsNullOrEmpty(currentPath))
            {
                System.Environment.SetEnvironmentVariable("PATH", executableDir, System.EnvironmentVariableTarget.Process);
                return;
            } // End if (string.IsNullOrEmpty(currentPath)) 

            // 2. Split the path variable into individual folders
            string[] paths = currentPath.Split(new[] { System.IO.Path.PathSeparator }, System.StringSplitOptions.RemoveEmptyEntries);
            bool alreadyExists = false;

            for (int i = 0; i < paths.Length; i++)
            {
                // Compare with an ordinal ignore case comparison to be robust against case variations
                if (string.Equals(paths[i], executableDir, System.StringComparison.OrdinalIgnoreCase))
                {
                    alreadyExists = true;
                    break;
                } // End if (string.Equals(paths[i], executableDir, System.StringComparison.OrdinalIgnoreCase))

            } // Next i 

            // 3. Append only if not already present
            if (!alreadyExists)
            {
                string newPath = currentPath + System.IO.Path.PathSeparator + executableDir;
                System.Environment.SetEnvironmentVariable("PATH", newPath, System.EnvironmentVariableTarget.Process);

                // To persist: 
                // System.Environment.SetEnvironmentVariable("PATH", newPath, System.EnvironmentVariableTarget.User);
                // Permissions: Unlike Machine variables,
                // setting User variables does not require
                // elevated administrator rights.
            } // End if (!alreadyExists) 

        } // End Sub AddToCurrentProcessPath 


        internal static System.Threading.Tasks.Task WaitForExitAsync(
            this System.Diagnostics.Process process
        )
        {
            System.Threading.Tasks.TaskCompletionSource<bool>
                tcs = new System.Threading.Tasks.TaskCompletionSource<bool>();

            System.EventHandler onExited = null;
            onExited = new System.EventHandler(
                delegate (object sender, System.EventArgs args)
                {
                    process.Exited -= onExited;
                    tcs.TrySetResult(true);
                } // End Delegate 
            );

            process.EnableRaisingEvents = true;
            process.Exited += onExited;

            // Double check in case the process exited before we attached the event
            if (process.HasExited)
            {
                process.Exited -= onExited;
                tcs.TrySetResult(true);
            } // End if (process.HasExited) 

            // if (process.HasExited) return System.Threading.Tasks.Task.CompletedTask;

            return tcs.Task;
        } // End Task WaitForExitAsync 


    } // End Class PipeRunnerHelper 


} // End Namespace 

