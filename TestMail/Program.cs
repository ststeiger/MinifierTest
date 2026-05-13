namespace TestMail
{

    internal class Program
    {


        internal static async System.Threading.Tasks.Task<int> Main(string[] args)
        {
            await GitCredentialHelper.TestAsync();

            await ImapEmailDownloader.DownloadEmailsAsync();
            await GraphEmailDownloader.DownloadEmailsAsync();
            await EwsEmailDownloader.DownloadEmailsAsync();

            await System.Console.Out.WriteLineAsync("-- - Press any key to continue --- ");
            return 0;
        } // End Task Main 


    } // End Class Program 


} // End Namespace 
