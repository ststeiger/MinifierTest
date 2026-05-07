
namespace MinifierTest
{


    public static class RedditImageDownloader
    {

        private static string ProjectDirectory
        {
            get
            {
                string bd = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..");
                return System.IO.Path.GetFullPath(bd);
            } // End Getter 
        } // End Property ProjectDirectory 


        private static void Base64ToImage()
        {
            // data:image/webp;base64,UklGRjoAAABXRUJQVlA4IC4AAACyAgCdASoCAAIALmk0mk0iIiIiIgBoSygABc6WWgAA/veff/0PP8bA//LwYAAA
            string base64 = "UklGRjoAAABXRUJQVlA4IC4AAACyAgCdASoCAAIALmk0mk0iIiIiIgBoSygABc6WWgAA/veff/0PP8bA//LwYAAA";
            byte[] ba = System.Convert.FromBase64String(base64);
            string outputPath = System.IO.Path.Combine(ProjectDirectory, "contentImage.webp");
            System.IO.File.WriteAllBytes(outputPath, ba);
        } // End Sub Base64ToImage 


        public static async System.Threading.Tasks.Task Test()
        {
            string url = "https://preview.redd.it/any-actual-agentic-autonomous-agents-out-there-v0-2i2ybase2r9e1.jpeg?width=1125&format=pjpg&auto=webp&s=b436f103293ecb4b93176d644b23b306e5b60b16";
            string outputPath = System.IO.Path.Combine(ProjectDirectory, "image.webp");

            System.Net.Http.HttpClientHandler handler =
                new System.Net.Http.HttpClientHandler()
                {
                    AutomaticDecompression =
                 System.Net.DecompressionMethods.GZip |
                 System.Net.DecompressionMethods.Deflate
                    //| System.Net.DecompressionMethods.Brotli // supports "br"
                };



            using (System.Net.Http.HttpClient client =
                new System.Net.Http.HttpClient(handler)
            )
            {
                // Set a browser-like User-Agent
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                    "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                // Optional: set referer (Reddit sometimes expects it)
                client.DefaultRequestHeaders.Referrer = new System.Uri("https://www.reddit.com/");

                
                // Accept header
                // if this is added, the image is lower DPI
                // client.DefaultRequestHeaders.Accept.ParseAdd("image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8");
                
                // Accept-Language
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9,de-CH;q=0.8,de;q=0.7");


                // DNT (Do Not Track)
                client.DefaultRequestHeaders.Add("DNT", "1");


                // No-cache headers
                client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue()
                {
                    NoCache = true
                };
                client.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");



                try
                {
                    using (System.Net.Http.HttpResponseMessage response =
                        await client.GetAsync(
                            url,
                            // Otherwise HttpClient buffers the whole response before you even get the stream—defeating the purpose.
                            System.Net.Http.HttpCompletionOption.ResponseHeadersRead
                        )
                    )
                    {
                        response.EnsureSuccessStatusCode();


                        // byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                        // await System.IO.File.WriteAllBytesAsync(outputPath, bytes);
                        // System.IO.File.WriteAllBytes(outputPath, bytes);
                        // await WriteAllBytesAsync(outputPath, bytes);

                        // using (System.IO.Stream stream = await response.Content.ReadAsStreamAsync())
                        // using (System.IO.FileStream fs = new System.IO.FileStream(
                        //         outputPath,
                        //         System.IO.FileMode.Create,
                        //         System.IO.FileAccess.Write,
                        //         System.IO.FileShare.None,
                        //         8192,
                        //         true
                        //     )
                        // )
                        // {
                        //     await stream.CopyToAsync(fs).ConfigureAwait(false);
                        // } // End Using fs 

                        using (System.IO.Stream input = await response.Content.ReadAsStreamAsync())
                        using (System.IO.FileStream output =
                            new System.IO.FileStream(
                                outputPath,
                                System.IO.FileMode.Create,
                                System.IO.FileAccess.Write,
                                System.IO.FileShare.None,
                                8192,
                                true
                            )
                        )
                        {
                            // For progress tracking
                            long totalRead = 0;
                            long? contentLength = response.Content.Headers.ContentLength;

                            // For buffering 
                            byte[] buffer = new byte[81920]; // 80 KB (same as .NET default)
                            int bytesRead;

                            while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await output.WriteAsync(buffer, 0, bytesRead);

                                // Add progress tracking
                                if (contentLength.HasValue)
                                {
                                    totalRead += bytesRead;
                                    double percent = (double)totalRead * 100.0 / contentLength.Value;
                                    System.Console.WriteLine($"{percent:F2}%");
                                } // End if (contentLength.HasValue) 
                            } // Whend 

                        } // End Using output 

                        System.Console.WriteLine("Image downloaded successfully!");
                    } // End Using response 

                } // End Try 
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Error: {ex.Message}");
                } // End Catch 

            } // End Using client 

        } // End Task Test 


        public static async System.Threading.Tasks.Task
            WriteAllBytesAsync(
            string path,
            byte[] bytes
        )
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(
                path,
                System.IO.FileMode.Create,
                System.IO.FileAccess.Write,
                System.IO.FileShare.None,
                bufferSize: 8192,
                useAsync: true))
            {
                await fs.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                await fs.FlushAsync();
            }

        } // End Task WriteAllBytesAsync 


    } // End Class RedditImageDownloader 


} // End Namespace 

