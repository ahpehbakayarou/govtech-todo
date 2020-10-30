using System;
using System.Collections;
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

        static void ThreadProc(Object data)
        {
            List<string> files;
            try {
                files = ((IEnumerable)data).Cast<string>().ToList();
                foreach (var dir in files)
                {
                    using var sr = new StreamReader(dir);
                    var content = sr.ReadToEnd();

                    if (content.Contains(KEYWORD))
                    {
                        Console.WriteLine($"dir:{dir}");
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("Unable to cast to List<string>: ", e.Message);
            }
        }

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
            Console.WriteLine($"Count:{files.Count}");
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

            /*
            await Task.Run(() => {
                ListDirectories(scanDir, files);
            });
            */

            Console.WriteLine($"Main thread: Finish scanning, start reading {files.Count} files.");
            await ReadFiles();

            /*
            Thread t1 = new Thread(ThreadProc);
            t1.Start(files);

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Main thread: Do some work.");
                Thread.Sleep(0);
            }

            t1.Join();
            */
            
            Console.WriteLine("Main thread: Scanning done. Please Enter key to exit.");
            Console.Read(); // Get string from user
            Environment.Exit(0);
        }

        static void ListDirectories(string currDir, List<string> list)
        {
            Console.WriteLine($"Scanning {currDir}");
            List<string> dirs = new List<string>(Directory.EnumerateFileSystemEntries(currDir)); 
            foreach (var dir in dirs)
            {
                if (IGNORE.Contains(dir.Substring(dir.Length - 4))) continue; //ignore file/folder name extension

                try
                {
                    if (!IsDirectory(dir))
                    {
                        if (!IsBinaryFile(dir))
                        {
                            // If file, check if it is binary or text file
                            //Console.WriteLine(IsBinaryFile(dir) ? $"{dir} is binary file" : $"{dir} is not binary file");
                            list.Add(dir);
                        }
                        continue;
                    }
                } catch
                {
                    //* Catch any exceptions and continue
                    continue;
                }

                ListDirectories(dir, list);
            }
        }

        private static bool IsDirectory(string path)
        {
            try
            {
                FileAttributes fa = File.GetAttributes(path);
                return (fa & FileAttributes.Directory) != 0;
            }
            catch
            {
                throw new Exception();
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