using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment
{
    class Program
    {
        static readonly string KEYWORD = "TODO";
        static readonly List<string> files = new List<string>();
        static string[] IGNORE;
        static string scanDir;

        static void ReadConfigSettings()
        {
            IDictionary<string, string> settings = new Dictionary<string, string>();
            using (var sr = new StreamReader("./Config.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] settingPair = line.Split('=');
                    settings.Add(settingPair[0], settingPair[1]);
                }
            }

            if (!settings.TryGetValue("Path", out scanDir))
            {
                Console.WriteLine("Error: Config does not contain the Path location to scan the files");
                Environment.Exit(0);
            };

            if (!Directory.Exists(scanDir)) {
                Console.WriteLine($"Error: Path location \"{scanDir}\" to scan the files does not exists");
                Environment.Exit(0);
            }

            if (settings.TryGetValue("Ignore", out string ignoreString))
            {
                IGNORE = ignoreString.Split(";");
            }
            else
            {
                Console.WriteLine("Error: Config does not contain the ignore list");
                Environment.Exit(0);
            }
        }

        static async Task ReadFiles()
        {
            //Console.WriteLine($"Count:{files.Count}");
            foreach (var dir in files)
            {
                using (var sr = new StreamReader(dir))
                {
                    var content = await sr.ReadToEndAsync();
                    if (content.Contains(KEYWORD))
                    {
                        Console.WriteLine($"dir:{dir}");
                    }
                }
            }
        }

        static async Task Main(string[] args)
        {
            // Get current directory
            // Loop to get all files under direcotry 
            // For each file in directory read file to find TODO           
            ReadConfigSettings();
            Console.WriteLine($"Main thread: Start scanning \"{scanDir}\".");
            ListDirectories(scanDir, files);

            Console.WriteLine($"Main thread: Finish scanning, start reading {files.Count} files.");
            await ReadFiles();
            
            Console.WriteLine("Main thread: Scanning done. Please Enter key to exit.");
            Console.Read(); // Get string from user
            Environment.Exit(0);
        }

        static void ListDirectories(string currDir, List<string> list)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(currDir);
            while (queue.Count > 0)
            {
                string path = queue.Dequeue();
                foreach (string subDir in Directory.GetDirectories(path))
                {
                    if (IGNORE.Contains(subDir.Substring(subDir.Length - 4))) continue; //ignore file/folder name extension

                    queue.Enqueue(subDir);
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                if (files != null)
                {
                    for (int i=0; i < files.Length; i++)
                    {
                        if (IGNORE.Contains(files[i].Substring(files[i].Length - 4))) continue; //ignore file/folder name extension

                        if (!IsBinaryFile(files[i]))
                        {
                            // If file, check if it is binary or text file
                            list.Add(files[i]);
                        }
                    }
                }
            }
        }

        private static bool IsBinaryFile(string path)
        {
            try
            {
                var content = File.ReadAllBytes(path);
                for (int i = 1; i < 512 && i < content.Length; i++)
                {
                    // Read the first 512 bytes to find consecutive nulls
                    if (content[i] == 0x00 && content[i - 1] == 0x00) return true;
                }

                return false;
            }
            catch
            {
                Console.WriteLine($"File read exception {path}");
                throw new Exception();
            }
        }
    }
}