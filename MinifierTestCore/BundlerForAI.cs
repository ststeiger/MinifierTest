
namespace MinifierTestCore
{ 


    public class BundlerForAI
    {

        // Define a delegate type (or just use System.Predicate<string>)
        public delegate bool FileFilterDelegate(string filePath);


        private static string ProjectDirectory
        {
            get
            {
                if(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", System.StringComparison.OrdinalIgnoreCase))
                    return System.IO.Path.GetFullPath(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", ".."));

                string bd = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
                return bd;
            } // End Getter 
        } // End Property ProjectDirectory 


        private static System.Collections.Generic.List<string> CombineMultipleLists(
            params System.Collections.Generic.ICollection<string>[] lists
        )
        {
            // 1. Calculate total capacity needed to optimize memory
            int totalCount = 0;
            foreach (System.Collections.Generic.ICollection<string> list in lists)
            {
                if (list != null) 
                    totalCount += list.Count;
            }

            // 2. Initialize list with the exact size required
            System.Collections.Generic.List<string> combined = new System.Collections.Generic.List<string>(totalCount);

            // 3. Add each list to the master list
            foreach (System.Collections.Generic.ICollection<string> list in lists)
            {
                if (list != null)
                    combined.AddRange(list);
            } // Next list 

            return combined;
        } // End Function CombineLists 




        private static readonly System.Collections.Generic.HashSet<string> ExcludedNames = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            ".env", ".env.local", ".env.production", ".env.development",
            "appsettings.Production.json", "secrets.json"
        };

        private static readonly System.Collections.Generic.HashSet<string> ExcludedExtensions = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            ".log", ".tmp", ".temp", ".cache", ".suo", ".user", ".pdb",
            ".exe", ".dll", ".so", ".dylib", ".pem", ".key", ".pfx",
            ".bak", ".backup", ".old"
        };

        private static readonly string[] ExcludedPathContains = new[]
        {
            "/bin/", "/obj/", "/.git/", "/node_modules/", "/.vs/",
            "/dist/", "/build/", "/Release/", "/Debug/", "/logs/",
            "/coverage/", "/test-results/", "/vendor/"
        };

        private static bool SafeFilter(string path)
        {
            string fileName = System.IO.Path.GetFileName(path);
            string ext = System.IO.Path.GetExtension(path);
            string lowerPath = path.Replace('\\', '/').ToLowerInvariant();

            // Name komplett ausschließen
            // Assuming ExcludedNames is a List or HashSet; using the instance method Contains
            if (ExcludedNames.Contains(fileName))
                return false;

            // Erweiterung ausschließen
            if (ExcludedExtensions.Contains(ext))
                return false;

            // Pfad enthält verbotene Ordner
            // Replaced .Any() with a foreach loop
            foreach (string forbidden in ExcludedPathContains)
            {
                if (lowerPath.Contains(forbidden))
                {
                    return false;
                }
            }

            // Versteckte Dateien/Ordner (außer .cs, .js etc.)
            if (fileName.StartsWith("."))
            {
                string[] allowedExtensions = new string[] { ".cs", ".js", ".ts", ".html", ".css" };
                bool isAllowedExt = false;

                // Replaced .Contains() with a manual loop
                foreach (string allowed in allowedExtensions)
                {
                    if (ext == allowed)
                    {
                        isAllowedExt = true;
                        break;
                    }
                }

                if (!isAllowedExt)
                    return false;
            }

            return true;
        }



        /// <summary>
        /// A Linux-secure file crawler that uses a delegate to decide which files to include.
        /// </summary>
        public static System.Collections.Generic.List<string> GetFilesCustom(
            string rootFolder, 
            FileFilterDelegate filter
        )
        {
            System.Collections.Generic.List<string> results = new System.Collections.Generic.List<string>();

            if (!System.IO.Directory.Exists(rootFolder))
                return results;

            // Get EVERYTHING first (using "*" to avoid Linux case-sensitivity issues with extensions)
            foreach (string filePath in System.IO.Directory.EnumerateFiles(rootFolder, "*", System.IO.SearchOption.AllDirectories))
            {
                // Invoke the delegate to see if this file should be included
                if (filter(filePath))
                    results.Add(filePath);
            } // Next filePath 

            results.Sort();

            return results;
        } // End Function GetFilesCustom 


        public async static System.Threading.Tasks.Task<string> BundleFiles(
            string sourceFolder,
            bool relativePaths,
            params System.Collections.Generic.ICollection<string>[] lists
        )
        {
            if (!System.IO.Directory.Exists(sourceFolder))
            {
                System.Console.WriteLine($"Folder does not exist: {sourceFolder}");
                return "";
            } // End if (!System.IO.Directory.Exists(sourceFolder)) 

            System.Collections.Generic.List<string> allFiles = CombineMultipleLists(lists);


            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("===== FILE BUNDLE FOR LLM =====");
            sb.AppendLine();

            foreach (string file in allFiles)
            {
                string relativePath = System.IO.Path.GetRelativePath(sourceFolder, file);
                string content = await System.IO.File.ReadAllTextAsync(file);
                string pathToOutput = relativePaths ? relativePath : file; 

                sb.AppendLine($"===== BEGIN FILE: {pathToOutput} =====");
                sb.AppendLine();

                sb.AppendLine(content);

                sb.AppendLine();
                sb.AppendLine($"===== END FILE: {pathToOutput} =====");
                sb.AppendLine();
            } // Next file 


            sb.AppendLine();
            sb.AppendLine("===== END FILE BUNDLE FOR LLM =====");
            sb.AppendLine();

            string bundleContent = sb.ToString();
            sb.Clear();

            return bundleContent;
        } // End Task BundleFiles 


        public async static System.Threading.Tasks.Task BundleMobile2()
        {
            // Usage:
            // dotnet run "C:\path\to\js-folder" "output.txt"
            string sourceFolder = @"D:\username\Documents\Visual Studio 2022\gitlab\TestPWA\TestPWA8\wwwroot\Checklist2\basicJS";
            string outputFile = System.IO.Path.Combine(ProjectDirectory, "bundle.txt");


            // List of files to exclude from the bundle(
            System.Collections.Generic.HashSet<string> jsExcludedSet = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
            {
                "html5-qrcode.min.js",
                "another-file.js",
                "secret-config.js"
            };

            FileFilterDelegate allJsFiles = delegate (string path)
            {
                string ext = System.IO.Path.GetExtension(path);
                string name = System.IO.Path.GetFileName(path);

                // 1. Check extension (Case-insensitive for Linux support)
                bool isJs = string.Equals(ext, ".js", System.StringComparison.OrdinalIgnoreCase);
                if (!isJs)
                    return false;

                if (jsExcludedSet.Contains(name)) 
                    return false;

                return true;
            };


            // System.Collections.Generic.List<string> htmlExcludedList = new System.Collections.Generic.List<string>();
            System.Collections.Generic.HashSet<string> htmlExcludedSet = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            FileFilterDelegate allHtmlFiles = delegate (string path)
            {
                string ext = System.IO.Path.GetExtension(path);
                string name = System.IO.Path.GetFileName(path);

                // 1. Check extension (Case-insensitive for Linux support)
                bool isHtml = string.Equals(ext, ".htm", System.StringComparison.OrdinalIgnoreCase);
                isHtml = isHtml | string.Equals(ext, ".html", System.StringComparison.OrdinalIgnoreCase);

                if (!isHtml)
                    return false;

                if (htmlExcludedSet.Contains(name)) 
                    return false;

                return true;
            };

            // Get all files initially
            System.Collections.Generic.List<string> jsFiles = GetFilesCustom(sourceFolder, allJsFiles);
            // System.Collections.Generic.List<string> htmlFiles = GetFilesCustom(sourceFolder, allHtmlFiles);

            string bundleCotnent = await BundleFiles(sourceFolder, false, jsFiles, new string[] {
                @"D:\username\Documents\Visual Studio 2022\TFS\COR-CAFM-V4\CAFM\CAFM\Modules\Mobile2\index.html"
            });

            await System.IO.File.WriteAllTextAsync(outputFile, bundleCotnent, System.Text.Encoding.UTF8);

            System.Globalization.CultureInfo culture = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberGroupSeparator = "'";
            System.Console.Write($"Bundled {jsFiles.Count} ({bundleCotnent.Length.ToString("N0", culture)} bytes) JS files into: ");
            System.Console.WriteLine(outputFile);
        } // End Task BundleMobile2 


        public async static System.Threading.Tasks.Task Test()
        {
            string sourceFolder = @"D:\Repositories\Projects\hmailserver\hmailserver\source\Server";
            string outputFile = System.IO.Path.Combine(ProjectDirectory, "bundle.txt");


            System.Collections.Generic.HashSet<string> excludedSet = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            FileFilterDelegate allFiles = delegate (string path)
            {
                string ext = System.IO.Path.GetExtension(path);
                string name = System.IO.Path.GetFileName(path);

                // 1. Check extension (Case-insensitive for Linux support)
                // bool isHtml = string.Equals(ext, ".htm", System.StringComparison.OrdinalIgnoreCase);
                // isHtml = isHtml | string.Equals(ext, ".html", System.StringComparison.OrdinalIgnoreCase);
                // if (!isHtml) return false;

                if (excludedSet.Contains(name))
                    return false;

                return true;
            };


            // Get all files initially
            System.Collections.Generic.List<string> âllFiles = GetFilesCustom(sourceFolder, allFiles);

            string bundleCotnent = await BundleFiles(sourceFolder, false, âllFiles);

            await System.IO.File.WriteAllTextAsync(outputFile, bundleCotnent, System.Text.Encoding.UTF8);

            System.Globalization.CultureInfo culture = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberGroupSeparator = "'";
            System.Console.Write($"Bundled {âllFiles.Count} ({bundleCotnent.Length.ToString("N0", culture)} bytes) JS files into: ");
            System.Console.WriteLine(outputFile);
        } // End Task Test 


    } // End Class BundlerForAI 


} // End Namespace 
