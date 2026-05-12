
namespace TestMail
{


    class GraphEmailDownloader
    {


        // If you only have an email account and do not have (or want to set up) an Azure App Registration with a clientId,
        // then Microsoft Graph is not the right choice for you.
        public static async System.Threading.Tasks.Task DownloadEmailsAsync()
        {
            // CONFIGURATION
            // Paste your Client ID from the Azure Portal here
            string clientId = "YOUR_CLIENT_ID_FROM_AZURE_PORTAL";
            string tenantId = "common"; // Works for both personal and work accounts usually

            // For personal accounts, you might need to use "organizations" or leave it blank depending on setup, 
            // but "common" is the safest starting point for scripts.

            string downloadDirectory = System.IO.Path.Combine(System.Environment.CurrentDirectory, "GraphEmails");

            if (!System.IO.Directory.Exists(downloadDirectory))
            {
                System.IO.Directory.CreateDirectory(downloadDirectory);
            }

            System.Console.WriteLine("Initializing Graph Client...");

            // 1. Create the Authentication Provider
            // Scopes: User.Read is needed to sign in, Mail.Read is needed to get emails
            string[] scopes = new[] { "User.Read", "Mail.Read" };

            // This will pop up a console interaction asking you to visit a website
            Azure.Identity.DeviceCodeCredentialOptions options = new Azure.Identity.DeviceCodeCredentialOptions()
            {
                AuthorityHost = Azure.Identity.AzureAuthorityHosts.AzurePublicCloud,
                ClientId = clientId,
                TenantId = tenantId,
                // Callback automatically prints the code to the console
                DeviceCodeCallback = async delegate(Azure.Identity.DeviceCodeInfo code, System.Threading.CancellationToken cancellation) 
                {
                    await System.Console.Out.WriteLineAsync(code.Message);
                },
            };

            Azure.Identity.DeviceCodeCredential deviceCodeCredential = new Azure.Identity.DeviceCodeCredential(options);
            Microsoft.Graph.GraphServiceClient graphClient = new Microsoft.Graph.GraphServiceClient(deviceCodeCredential, scopes);

            try
            {
                await System.Console.Out.WriteLineAsync("Attempting to fetch messages...");

                // 1. Define the initial request
                System.Threading.Tasks.Task<Microsoft.Graph.Models.MessageCollectionResponse?> mailRequest = graphClient.Me.Messages
                    .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Top = 25;
                        requestConfiguration.QueryParameters.Select = new[] { "id", "subject" };
                        requestConfiguration.Headers.Add("Prefer", "outlook.body-content-type=\"text\"");
                    });

                // 2. Get the initial page
                Microsoft.Graph.Models.MessageCollectionResponse? initialPage = await mailRequest;

                // -------------------------------------------------------------
                // FIX 2: PageIterator Setup
                // -------------------------------------------------------------

                if (initialPage != null)
                {
                    // PageIterator requires <TEntity, TCollectionPage>
                    // The collection response for Messages is MessageCollectionResponse
                    Microsoft.Graph.PageIterator<Microsoft.Graph.Models.Message, Microsoft.Graph.Models.MessageCollectionResponse> pageIterator = Microsoft.Graph.PageIterator<Microsoft.Graph.Models.Message, Microsoft.Graph.Models.MessageCollectionResponse>.CreatePageIterator(
                        graphClient,
                        initialPage,
                        // Callback for each message
                        async delegate (Microsoft.Graph.Models.Message m)
                        {
                            try
                            {
                                // Get raw MIME content
                                System.IO.Stream? messageStream = await graphClient.Me.Messages[m.Id].Content.GetAsync();

                                string safeSubject = "";

                                if (string.IsNullOrWhiteSpace(safeSubject))
                                    safeSubject = "NoSubject";
                                else if (m.Subject != null)
                                    safeSubject = string.Join("_", m.Subject.Split(System.IO.Path.GetInvalidFileNameChars()));

                                string fileName = $"{m.Id}_{safeSubject}.eml";
                                string filePath = System.IO.Path.Combine(downloadDirectory, fileName);

                                // Save to file
                                if (messageStream != null)
                                    using (System.IO.FileStream fs = System.IO.File.Create(filePath))
                                    {
                                        messageStream.CopyTo(fs);
                                    } // End Using fs 

                                await System.Console.Out.WriteLineAsync($"Downloaded: {safeSubject}");
                                return true; // Return true to continue
                            } // End Try 
                            catch (System.Exception ex)
                            {
                                await System.Console.Out.WriteLineAsync($"Failed to download {m.Id}: {ex.Message}");
                                return true; // Return true to continue even if one fails
                            } // End Catch 
                        } // End Delegate 
                    );

                    // 3. Run the iterator
                    await pageIterator.IterateAsync();
                }

                await System.Console.Out.WriteLineAsync("All emails downloaded.");
            } // End Catch 
            catch (Microsoft.Graph.Models.ODataErrors.ODataError odataError)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                await System.Console.Out.WriteLineAsync($"Graph API Error: {odataError.Error?.Code} - {odataError.Error?.Message}");
                System.Console.ResetColor();
            } // End Catch odataError  
            catch (System.Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                await System.Console.Out.WriteLineAsync($"General Error: {ex.Message}");
                System.Console.ResetColor();
            } // End Catch ex 

        } // End Task DownloadEmailsAsync 


    } // End Class GraphEmailDownloader 


} // End Namespace 
