
namespace TestMail
{


    public class EwsEmailDownloader
    {


        public static async System.Threading.Tasks.Task DownloadEmailsAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            // KONFIGURATION
            string email = "your_email@outlook.com";
            // App-Passwort zwingend erforderlich bei 2FA
            string password = "your_app_password";

            string downloadDirectory = System.IO.Path.Combine(System.Environment.CurrentDirectory, "EwsEmails");

            if (!System.IO.Directory.Exists(downloadDirectory))
            {
                System.IO.Directory.CreateDirectory(downloadDirectory);
            }

            // 1. Service Initialisierung
            Microsoft.Exchange.WebServices.Data.ExchangeService service =
                new Microsoft.Exchange.WebServices.Data.ExchangeService(Microsoft.Exchange.WebServices.Data.ExchangeVersion.Exchange2013_SP1);

            service.Credentials = new Microsoft.Exchange.WebServices.Data.WebCredentials(email, password);


            string url = "https://outlook.office365.com/EWS/Exchange.asmx";
            url = "https://yourcompany.com/EWS/Exchange.asmx";

            service.Url = new System.Uri(url);

            try
            {
                // 2. Bind zum Inbox-Ordner (Korrektur: Kapselung in Task.Run für echte Asynchronität)
                Microsoft.Exchange.WebServices.Data.Folder? inbox = await System.Threading.Tasks.Task.Run(() =>
                    Microsoft.Exchange.WebServices.Data.Folder.Bind(service, Microsoft.Exchange.WebServices.Data.WellKnownFolderName.Inbox),
                    cancellationToken);

                if (inbox == null) return;

                System.Console.WriteLine($"Gefunden: {inbox.TotalCount} Nachrichten.");

                int pageSize = 50;
                int offset = 0;
                bool moreItems = true;

                while (moreItems)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Microsoft.Exchange.WebServices.Data.ItemView view = new Microsoft.Exchange.WebServices.Data.ItemView(pageSize, offset, Microsoft.Exchange.WebServices.Data.OffsetBasePoint.Beginning);
                    view.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(
                        Microsoft.Exchange.WebServices.Data.BasePropertySet.IdOnly,
                        Microsoft.Exchange.WebServices.Data.ItemSchema.Subject
                    );

                    // 3. FindItems Korrektur (Kapselung in Task.Run)
                    var findResults = await System.Threading.Tasks.Task.Run(() =>
                        service.FindItems(inbox.Id, view),
                        cancellationToken);

                    foreach (var item in findResults.Items)
                    {
                        if (item is Microsoft.Exchange.WebServices.Data.EmailMessage message)
                        {
                            // Volle Nachricht laden (inkl. MimeContent)
                            await System.Threading.Tasks.Task.Run(() =>
                                message.Load(new Microsoft.Exchange.WebServices.Data.PropertySet(Microsoft.Exchange.WebServices.Data.ItemSchema.MimeContent)),
                                cancellationToken);

                            string subject = message.Subject ?? "NoSubject";
                            string safeSubject = CleanFileName(subject);
                            string shortId = message.Id.UniqueId.Substring(0, System.Math.Min(10, message.Id.UniqueId.Length));

                            string fileName = $"{shortId}_{safeSubject}.eml";
                            string filePath = System.IO.Path.Combine(downloadDirectory, fileName);

                            if (System.IO.File.Exists(filePath)) continue;

                            // Datei schreiben (Asynchron)
                            if (message.MimeContent?.Content != null)
                            {
                                await System.IO.File.WriteAllBytesAsync(filePath, message.MimeContent.Content, cancellationToken);
                                System.Console.WriteLine($"Heruntergeladen: {subject}");
                            }
                        }
                    }

                    offset += pageSize;
                    moreItems = findResults.MoreAvailable;
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"EWS Fehler: {ex.Message}");
            }
        }

        private static string CleanFileName(string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName.Length > 100 ? fileName.Substring(0, 100) : fileName;
        }
    }
}