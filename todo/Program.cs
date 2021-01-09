using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Assignment.Enums;

namespace Assignment
{
    public class Enums
    {
        public enum ReadConfigStatus : ushort
        {
            CONFIG_FILE_NOT_FOUND = 1,
            PATH_NOT_FOUND,
            IGNORE_NOT_FOUND,
            OK,
        }
    }

    public class Program
    {
        static readonly string KEYWORD = "TODO";
        static readonly List<string> files = new List<string>();
        static string[] IGNORE;
        static string scanDir;
        static readonly string CONFIG_FILE = "./Config.txt";

        //* -----------------------------------------------------------------------------------------------
        //* Public
        //* -----------------------------------------------------------------------------------------------
        public static bool ChangeDirectory(string input)
        {
            if (input == "")
            {
                scanDir = Directory.GetCurrentDirectory();
                WriteConfigSetting("Path", scanDir);
            }
            else if (!Directory.Exists(input))
            {
                Console.WriteLine("Directory not found");
                return false;
            }
            else
            {
                scanDir = input;
                WriteConfigSetting("Path", scanDir);
            }

            return true;
        }

        public static ReadConfigStatus ReadConfigSettings(string configFile = "./Config.txt")
        {
            if (!File.Exists(configFile))
            {
                Console.WriteLine($"Error: Config File not found");
                return ReadConfigStatus.CONFIG_FILE_NOT_FOUND;
            }

            IDictionary<string, string> settings = new Dictionary<string, string>();
            using (var sr = new StreamReader(configFile))
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
                return ReadConfigStatus.PATH_NOT_FOUND;
            };

            if (!Directory.Exists(scanDir)) {
                Console.WriteLine($"Error: Path location \"{scanDir}\" to scan the files does not exists");
                return ReadConfigStatus.PATH_NOT_FOUND;
            }

            if (settings.TryGetValue("Ignore", out string ignoreString))
            {
                IGNORE = ignoreString.Split(";");
            }
            else
            {
                Console.WriteLine("Error: Config does not contain the ignore list");
                return ReadConfigStatus.IGNORE_NOT_FOUND;
            }

            return ReadConfigStatus.OK;
        }

        //* -----------------------------------------------------------------------------------------------
        //* Private
        //* -----------------------------------------------------------------------------------------------

        static void WriteConfigSetting(string key, string value)
        {
            string[] arrLine = File.ReadAllLines(CONFIG_FILE);
            for (int i=0; i<arrLine.Length; i++)
            {
                string[] settingPair = arrLine[i].Split('=');
                if (key == settingPair[0])
                {
                    settingPair[1] = value;
                    arrLine[i] = String.Join("=", settingPair);
                    break;
                }
            }
            File.WriteAllLines(CONFIG_FILE, arrLine);
        }

        static async Task ReadFiles()
        {
            foreach (var dir in files)
            {
                using (var sr = new StreamReader(dir))
                {
                    var content = await sr.ReadToEndAsync();
                    if (content.Contains(KEYWORD))
                    {
                        Console.WriteLine($"Found :{dir}");
                    }
                }
            }
        }

        static async Task Main(string[] args)
        {
            // Get current directory
            // Loop to get all files under direcotry 
            // For each file in directory read file to find TODO
            if (ReadConfigSettings() != ReadConfigStatus.OK)
            {
                Environment.Exit(0);
            }

            string input;
            do
            {
                Console.WriteLine("1. Change scan directory");
                Console.WriteLine($"2. Scan directory ({scanDir})");
                Console.WriteLine("3. Exit");
                Console.Write("Enter choice: ");

                input = Console.ReadLine();
                try
                {
                    int choice = Convert.ToInt32(input);
                    switch(choice)
                    {
                        case 1:
                            {
                                Console.Write("Enter directory (blank set to current directory): ");
                                string inputDirectory = Console.ReadLine();
                                ChangeDirectory(inputDirectory);
                                break;
                            }
                        case 2:
                            {
                                Console.WriteLine($"Start scanning \"{scanDir}\".");
                                ListDirectories(scanDir, files);
                                Console.WriteLine($"Finish scanning, start reading {files.Count} files.");
                                await ReadFiles();

                                Console.WriteLine("Scanning completed.");
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid choice");
                }
                
            } while (input != "3");

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

        static bool IsBinaryFile(string path)
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