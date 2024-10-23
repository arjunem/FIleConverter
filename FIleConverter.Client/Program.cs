using FIleConverter.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FileConverter.Client
{
    class Program
    {
        private static string? serverIp;
        private static int? port;
        static void Main(string[] args)
        {

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Define the path to your config.json file
            string configFilePath = Path.Combine(AppContext.BaseDirectory, "config.json");

            // Read the content of the config.json file
            string json = File.ReadAllText(configFilePath);

            // Deserialize the JSON content into C# objects
            var config = JsonConvert.DeserializeObject<AppConfig>(json);

            // Set the folder to monitor
            var folderToWatch = config.ApplicationSettings.FolderToWatch ?? configuration["ApplicationSettings:FolderToWatch"]; // Change to the folder you want to monitor

            serverIp = config.ApplicationSettings.ServerIP?? configuration["ApplicationSettings:ServerIP"];
            port = config.ApplicationSettings.Port;
            if (serverIp == null)
            {
                if(int.TryParse(configuration["ApplicationSettings:Port"]?.ToString(), out int output)){
                    port = output;
                }
            }

            // Create a FileSystemWatcher to monitor the directory
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path =  folderToWatch;

            // Watch for changes in the creation of new files
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime;

            // Filter to only watch for file creation (can be adjusted as needed)
            watcher.Filter = "*.csv*"; // Watch for all file types

            // Add event handler for the Created event (file added)
            watcher.Created += OnNewFileAdded;

            // Begin watching
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Monitoring folder: {folderToWatch}. Press 'q' to quit.");

            TCPFileClient.ReadHeartBeat(serverIp, port);

            // Keep the console app running until the user quits
            while (Console.Read() != 'q') ;
        }

        // Event handler for file created
        private static void OnNewFileAdded(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(100);
            TCPFileClient.SendFile(e.FullPath,serverIp,port);
        }
    }
}
