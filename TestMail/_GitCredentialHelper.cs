
namespace TestMail
{


    public class GitCredentialHelper
    {


        internal static async System.Threading.Tasks.Task TestAsync()
        {
            await AddCredentials("https", "gitlab.com", "FOOBAR2000", "NEW_GENERATED_TOKEN");
            await RemoveCredentials("https", "gitlab.com", "FOOBAR2000");
        } // End Task TestAsync 


        /// <summary>
        /// Approves and stores a Git credential.
        /// </summary>
        public static async System.Threading.Tasks.Task AddCredentials(
            string protocol, 
            string host, 
            string username, 
            string password
        )
        {
            // The credential payload. 
            // Note the double newline (\n\n) at the end! 
            // Git requires a blank line to know the input is finished.
            string credentials = $"protocol={protocol}\n" +
                     $"host={host}\n" +
                     $"username={username}\n" +
                     $"password={password}\n\n";


            System.Text.UTF8Encoding utf8NoBom = new System.Text.UTF8Encoding(false);


            System.Diagnostics.ProcessStartInfo startInfo =
                new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "git",
                    Arguments = "credential approve",
                    RedirectStandardInput = true,   // Required to write to the process
                    RedirectStandardOutput = true, // Required to read output/prevent console popup
                    RedirectStandardError = true, // Required to read errors
                    UseShellExecute = false,     // Must be false to redirect streams
                    CreateNoWindow = true,      // Prevents a command prompt window from flashing

                    StandardInputEncoding = utf8NoBom,
                    StandardOutputEncoding = utf8NoBom,
                    StandardErrorEncoding = utf8NoBom
                };

            using (System.Diagnostics.Process process =
                new System.Diagnostics.Process()
                {
                    StartInfo = startInfo
                }
            )
            {
                process.Start();

                // Write the credentials to the process's standard input
                await process.StandardInput.WriteAsync(credentials);

                // Crucial: Close the input stream so Git knows we are done sending data
                process.StandardInput.Close();

                System.Threading.Tasks.Task processExit = process.WaitForExitAsync();

                // Read the output and errors (prevents deadlocking if Git writes a lot)
                System.Threading.Tasks.Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                System.Threading.Tasks.Task<string> errorTask = process.StandardError.ReadToEndAsync();

                // Now await the reading tasks to get the actual data
                // (The tasks are likely already finished by the time the process exits)
                string output = await outputTask;
                string error = await errorTask;


                // Wait for the process to exit
                await processExit;

                if (process.ExitCode == 0)
                {
                    await System.Console.Out.WriteLineAsync("Credential approved successfully.");
                }
                else
                {
                    await System.Console.Error.WriteLineAsync($"Failed to approve credential. Exit Code: {process.ExitCode}");
                    if (!string.IsNullOrEmpty(error))
                    {
                        await System.Console.Error.WriteLineAsync($"Git Error: {error}");
                    }
                }

            } // End Using process 

        } // End Task AddCredentials 


        /// <summary>
        /// Rejects and removes a Git credential. (Password is not required for rejection).
        /// 
        /// Note:
        /// This removes the token from your local Git credential store
        /// (like Windows Credential Manager, macOS Keychain, or the store file).
        /// If the token itself has been compromised, you should still
        /// revoke/regenerate it directly in the GitLab settings.
        /// </summary>
        public static async System.Threading.Tasks.Task RemoveCredentials(
            string protocol, 
            string host, 
            string username
        )
        {
            // The credential payload. 
            // Note the double newline (\n\n) at the end! 
            // Git requires a blank line to know the input is finished.
            string credentials = $"protocol={protocol}\n" +
                     $"host={host}\n" +
                     $"username={username}\n\n";

            System.Text.UTF8Encoding utf8NoBom = new System.Text.UTF8Encoding(false);


            System.Diagnostics.ProcessStartInfo startInfo =
                new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "git",
                    Arguments = "credential reject",
                    RedirectStandardInput = true,   // Required to write to the process
                    RedirectStandardOutput = true, // Required to read output/prevent console popup
                    RedirectStandardError = true, // Required to read errors
                    UseShellExecute = false,     // Must be false to redirect streams
                    CreateNoWindow = true,      // Prevents a command prompt window from flashing

                    StandardInputEncoding = utf8NoBom,
                    StandardOutputEncoding = utf8NoBom,
                    StandardErrorEncoding = utf8NoBom
                };

            using (System.Diagnostics.Process process =
                new System.Diagnostics.Process()
                {
                    StartInfo = startInfo
                }
            )
            {
                process.Start();

                // Write the credentials to the process's standard input
                await process.StandardInput.WriteAsync(credentials);

                // Crucial: Close the input stream so Git knows we are done sending data
                process.StandardInput.Close();

                System.Threading.Tasks.Task processExit = process.WaitForExitAsync();

                // Read the output and errors (prevents deadlocking if Git writes a lot)
                System.Threading.Tasks.Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                System.Threading.Tasks.Task<string> errorTask = process.StandardError.ReadToEndAsync();

                // Now await the reading tasks to get the actual data
                // (The tasks are likely already finished by the time the process exits)
                string output = await outputTask;
                string error = await errorTask;


                // Wait for the process to exit
                await processExit;

                if (process.ExitCode == 0)
                {
                    await System.Console.Out.WriteLineAsync("Credential removed successfully.");
                }
                else
                {
                    await System.Console.Error.WriteLineAsync($"Failed to reject credential. Exit Code: {process.ExitCode}");
                    if (!string.IsNullOrEmpty(error))
                    {
                        await System.Console.Error.WriteLineAsync($"Git Error: {error}");
                    }
                }

            } // End Using process 

        } // End Task RemoveCredentials 


    } // End Class 


} // End Namespace 