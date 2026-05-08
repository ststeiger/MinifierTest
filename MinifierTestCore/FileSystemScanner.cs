
namespace MinifierTestCore
{


    public static class FileSystemScanner
    {


        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesRecursive(string rootPath)
        {
            // 1. Enumerate files in the current (top) directory
            foreach (string file in System.IO.Directory.EnumerateFiles(rootPath))
            {
                yield return file;
            }

            // 2. Enumerate subdirectories and recurse into them
            foreach (string dir in System.IO.Directory.EnumerateDirectories(rootPath))
            {
                // Use a try-catch here if you want to skip folders with no access
                foreach (string file in EnumerateFilesRecursive(dir))
                {
                    yield return file;
                }
            }
        } // End Generator EnumerateFilesRecursive 


        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesIterative(string rootPath)
        {
            System.Collections.Generic.Stack<string> stack = new System.Collections.Generic.Stack<string>();
            stack.Push(rootPath);

            while (stack.Count > 0)
            {
                string currentDir = stack.Pop();

                // 1. Enumerate files in the current directory
                System.Collections.Generic.IEnumerable<string> files = null;
                try
                {
                    files = System.IO.Directory.EnumerateFiles(currentDir);
                }
                catch (System.UnauthorizedAccessException)
                {
                    // Skip folders we can't open
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string file in files)
                {
                    yield return file;
                }

                // 2. Push subdirectories onto the stack for later processing
                try
                {
                    foreach (string dir in System.IO.Directory.EnumerateDirectories(currentDir))
                    {
                        stack.Push(dir);
                    }
                }
                catch (System.UnauthorizedAccessException)
                { } // Already handled above or skip
            } // Whend 

        } // End Generator EnumerateFilesIterative 



        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesRecursiveOrdered(string rootPath)
        {
            System.Collections.Generic.Stack<string> stack = new System.Collections.Generic.Stack<string>();
            stack.Push(rootPath);

            while (stack.Count > 0)
            {
                string currentDir = stack.Pop();

                // 1. Process files in the current folder first
                System.Collections.Generic.IEnumerable<string> files;
                try
                {
                    files = System.IO.Directory.EnumerateFiles(currentDir);
                }
                catch (System.UnauthorizedAccessException)
                {
                    continue;
                }

                foreach (string file in files)
                    yield return file;

                // 2. Get subdirectories
                try
                {
                    string[] subDirs = System.IO.Directory.GetDirectories(currentDir);

                    // 3. To preserve order (A then B then C), 
                    // we must push them onto the stack in REVERSE (C, then B, then A).
                    for (long i = subDirs.Length - 1; i > -1; i--)
                        stack.Push(subDirs[i]);
                }
                catch (System.UnauthorizedAccessException)
                { }
            }
        } // End Generator EnumerateFilesRecursiveOrdered 


        // This does not 
        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesQueueReorder(string rootPath)
        {
            System.Collections.Generic.Queue<string> queue = new System.Collections.Generic.Queue<string>();
            queue.Enqueue(rootPath);

            while (queue.Count > 0)
            {
                string currentDir = queue.Dequeue();

                // 1. Process files in current directory
                System.Collections.Generic.IEnumerable<string> files = null;
                try
                {
                    files = System.IO.Directory.EnumerateFiles(currentDir);
                }
                catch (System.UnauthorizedAccessException)
                {
                    continue;
                }

                foreach (string file in files)
                {
                    yield return file;
                }

                // 2. Add subdirectories to the queue
                try
                {
                    foreach (string dir in System.IO.Directory.EnumerateDirectories(currentDir))
                    {
                        // No need to reverse; they will be processed in the order found
                        queue.Enqueue(dir);
                    }
                }
                catch (System.UnauthorizedAccessException)
                { }
            } // Whend 
        } // End Generator EnumerateFilesQueue 


        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesLinkedList(string rootPath)
        {
            System.Collections.Generic.LinkedList<string> list = new System.Collections.Generic.LinkedList<string>();
            list.AddFirst(rootPath);

            while (list.Count > 0)
            {
                // Always take from the front
                string currentDir = list.First.Value;
                list.RemoveFirst();

                // 1. Process files in current directory
                System.Collections.Generic.IEnumerable<string> files;
                try
                {
                    files = System.IO.Directory.EnumerateFiles(currentDir);
                }
                catch (System.UnauthorizedAccessException)
                {
                    continue;
                }

                foreach (string file in files)
                {
                    yield return file;
                }

                // 2. Get subdirectories
                try
                {
                    string[] subDirs = System.IO.Directory.GetDirectories(currentDir);

                    // To keep the order "A, B, C", we push them to the front 
                    // in reverse: C, then B, then A. 
                    // Now A is at the very front of the list.
                    for (int i = subDirs.Length - 1; i >= 0; i--)
                    {
                        list.AddFirst(subDirs[i]);
                    }
                }
                catch (System.UnauthorizedAccessException)
                { }
            } // Whend 

        } // End Generator EnumerateFilesLinkedList 


        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesLinkedListOrder(string rootPath)
        {
            System.Collections.Generic.LinkedList<string> list = new System.Collections.Generic.LinkedList<string>();
            // Start with the root
            System.Collections.Generic.LinkedListNode<string> currentNode = list.AddFirst(rootPath);

            while (currentNode != null)
            {
                string path = currentNode.Value;

                // 1. Process files in the current directory
                System.Collections.Generic.IEnumerable<string> files;
                try
                {
                    files = System.IO.Directory.EnumerateFiles(path);
                }
                catch (System.UnauthorizedAccessException)
                {
                    System.Collections.Generic.LinkedListNode<string>? next = currentNode.Next;
                    list.Remove(currentNode);
                    currentNode = next;
                    continue;
                }

                foreach (string file in files)
                {
                    yield return file;
                }

                // 2. Insert subdirectories immediately AFTER the current node
                try
                {
                    // We keep a reference to where we are inserting 
                    // so the subfolders stay in their original order (A, B, C)
                    System.Collections.Generic.LinkedListNode<string> insertionPoint = currentNode;

                    foreach (string dir in System.IO.Directory.EnumerateDirectories(path))
                    {
                        insertionPoint = list.AddAfter(insertionPoint, dir);
                    }
                }
                catch (System.UnauthorizedAccessException)
                { }

                // 3. Move to the next node in the list and remove the one we just finished
                System.Collections.Generic.LinkedListNode<string> processedNode = currentNode;
                currentNode = currentNode.Next;
                list.Remove(processedNode);
            }
        }


        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesNoReverse(string rootPath)
        {
            System.Collections.Generic.Stack<System.Collections.Generic.IEnumerator<string>> stack = new System.Collections.Generic.Stack<System.Collections.Generic.IEnumerator<string>>();

            // Start with the root files
            foreach (string file in System.IO.Directory.EnumerateFiles(rootPath))
                yield return file;

            // Get the first level of directories
            stack.Push(System.IO.Directory.EnumerateDirectories(rootPath).GetEnumerator());

            while (stack.Count > 0)
            {
                System.Collections.Generic.IEnumerator<string> currentEnum = stack.Peek();

                if (currentEnum.MoveNext())
                {
                    string currentDir = currentEnum.Current;

                    // Yield files in this directory
                    foreach (string file in System.IO.Directory.EnumerateFiles(currentDir))
                    {
                        yield return file;
                    }

                    // Dive into this directory's subdirectories
                    stack.Push(System.IO.Directory.EnumerateDirectories(currentDir).GetEnumerator());
                }
                else
                {
                    // No more directories at this level, pop and go up
                    currentEnum.Dispose();
                    stack.Pop();
                }
            }
        }


        /// <summary>
        /// Enumerates files recursively using a stack of enumerators.
        /// This preserves depth-first order without reversing arrays and handles permissions gracefully.
        /// </summary>
        public static System.Collections.Generic.IEnumerable<string> EnumerateFilesSafe(
            string rootPath
        )
        {
            if (string.IsNullOrWhiteSpace(rootPath)) 
                throw new System.ArgumentNullException(nameof(rootPath));

            if (!System.IO.Directory.Exists(rootPath)) 
                yield break;

            System.Collections.Generic.Stack<System.Collections.Generic.IEnumerator<string>> stack = 
                new System.Collections.Generic.Stack<System.Collections.Generic.IEnumerator<string>>();

            try
            {
                // 1. Process root files
                System.Collections.Generic.IEnumerator<string>? rootFiles = null;
                try
                {
                    rootFiles = System.IO.Directory.EnumerateFiles(rootPath).GetEnumerator();
                }
                catch (System.UnauthorizedAccessException) 
                { 
                    // Root inaccessible 
                }

                if (rootFiles != null)
                {
                    using (rootFiles)
                    {
                        while (rootFiles.MoveNext()) yield return rootFiles.Current;
                    }
                }

                // 2. Start directory recursion
                System.Collections.Generic.IEnumerator<string> rootDirs = null;
                try
                {
                    rootDirs = System.IO.Directory.EnumerateDirectories(rootPath).GetEnumerator();
                }
                catch (System.UnauthorizedAccessException) 
                { 
                    // Root dirs inaccessible 
                }

                if (rootDirs != null)
                    stack.Push(rootDirs);

                while (stack.Count > 0)
                {
                    System.Collections.Generic.IEnumerator<string> currentEnum = stack.Peek();
                    string? currentDir = null;

                    // Move to next directory at this level
                    try
                    {
                        if (currentEnum.MoveNext())
                        {
                            currentDir = currentEnum.Current;
                        }
                        else
                        {
                            currentEnum.Dispose();
                            stack.Pop();
                            continue;
                        }
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        currentEnum.Dispose();
                        stack.Pop();
                        continue;
                    }

                    // Yield files in the discovered directory
                    System.Collections.Generic.IEnumerator<string>? filesInDir = null;
                    try
                    {
                        filesInDir = System.IO.Directory.EnumerateFiles(currentDir).GetEnumerator();
                    }
                    catch (System.UnauthorizedAccessException) 
                    { }

                    if (filesInDir != null)
                    {
                        using (filesInDir)
                        {
                            while (filesInDir.MoveNext()) 
                                yield return filesInDir.Current;
                        }
                    }

                    // Push subdirectories onto the stack to dive deeper
                    System.Collections.Generic.IEnumerator<string>? subDirs = null;
                    try
                    {
                        subDirs = System.IO.Directory.EnumerateDirectories(currentDir).GetEnumerator();
                    }
                    catch (System.UnauthorizedAccessException) 
                    { }

                    if (subDirs != null)
                        stack.Push(subDirs);
                }
            }
            finally
            {
                // Crucial: Dispose all remaining enumerators if the user stops iterating early
                while (stack.Count > 0)
                {
                    stack.Pop()?.Dispose();
                }
            }
        }


    } // End Class FileSystemScanner


} // End Namespace 
