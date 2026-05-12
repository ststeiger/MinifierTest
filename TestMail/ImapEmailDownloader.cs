namespace TestMail
{

    // Important Note on Passwords:
    // If you have 2-Factor Authentication(2FA) enabled on your Outlook/Microsoft account (which you should),
    // you cannot use your standard login password.
    // You must generate an "App Password" in your Microsoft Account Security settings
    // and use that 16-character code in the password field below.

    // Method 1: Direct Link
    // The fastest way is to use the direct URL for App Passwords:
    // Go to: https://account.live.com/proofs/AppPassword
    // Sign in if prompted.
    // Click "New" (or "Create").
    // Give it a name(e.g., "CSharp Script").
    // Copy the code it generates immediately.You won't be able to see it again.


    // Method 2: Manual Navigation
    // If the direct link doesn't work, navigate manually:
    // Go to account.microsoft.com and sign in.
    // Click on the Security tab(top of the page).
    // Look for a section called "Advanced security options" and click it.
    // Under "Two-step verification," look for a link or section that says "App passwords" and click it.
    // Click "Create a new app password".
    // Copy the password generated.
    public class ImapEmailDownloader
    {
        public static async System.Threading.Tasks.Task DownloadEmailsAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            // CONFIGURATION
            // If you have Two-Factor Authentication (2FA) enabled, your regular password will never work for IMAP.
            // Modern email providers often block standard passwords for third-party apps to protect your account.
            string email = "your_email@outlook.com";
            string password = "your_app_password";
            string imapServer = "outlook.office365.com";
            int imapPort = 993;
            string downloadDirectory = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Emails");

            // Ensure download directory exists
            if (!System.IO.Directory.Exists(downloadDirectory))
            {
                System.IO.Directory.CreateDirectory(downloadDirectory);
            }

            using (MailKit.Net.Imap.ImapClient client = new MailKit.Net.Imap.ImapClient())
            {
                try
                {
                    await client.ConnectAsync(imapServer, imapPort, MailKit.Security.SecureSocketOptions.SslOnConnect, cancellationToken);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(email, password, cancellationToken);

                    // Korrektur: Nullable-Typ für IMailFolder
                    MailKit.IMailFolder? inbox = client.Inbox;

                    // Sicherer Null-Check vor dem Öffnen
                    if (inbox != null)
                    {
                        await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly, cancellationToken);
                        System.Console.WriteLine($"Gefunden: {inbox.Count} Nachrichten.");

                        for (int i = inbox.Count - 1; i >= 0; i--)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            // Korrektur: Verwenden von FetchAsync anstelle von GetMessageSummaryAsync 
                            // Wir fordern die UIDs und den Envelope (Metadaten) an
                            MailKit.FetchRequest request = new MailKit.FetchRequest(MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Envelope);
                            System.Collections.Generic.IList<MailKit.IMessageSummary> summaries = await inbox.FetchAsync(new[] { i }, request, cancellationToken);
                            MailKit.IMessageSummary? summary = summaries.Count > 0 ? summaries[0] : null;

                            if (summary != null)
                            {
                                // Sicherer Umgang mit Nullable-Eigenschaften
                                string subject = summary.Envelope?.Subject ?? "NoSubject";
                                string safeSubject = CleanFileName(subject);
                                string uid = summary.UniqueId.ToString();

                                string fileName = $"{safeSubject}_{uid}.eml";
                                string filePath = System.IO.Path.Combine(downloadDirectory, fileName);

                                if (System.IO.File.Exists(filePath)) 
                                    continue;

                                using (MimeKit.MimeMessage message = await inbox.GetMessageAsync(i, cancellationToken))
                                {
                                    using (System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None, 4096, true))
                                    {
                                        await message.WriteToAsync(stream, cancellationToken);
                                    } // End Using stream 
                                } // End Using message 
                            } // End if (summary != null) 

                            if (i % 10 == 0) 
                                System.Console.WriteLine($"{inbox.Count - i} verarbeitet...");
                        } // Next i 

                    } // End if (inbox != null) 

                    await client.DisconnectAsync(true, cancellationToken);
                } // End Try 
                catch (System.OperationCanceledException) 
                { 
                    // Handle cancellation 
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Fehler: {ex.Message}");
                    throw;
                }
            }
        }

        private static string CleanFileName(string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName.Length > 120 ? fileName.Substring(0, 120) : fileName;
        }
    }
}