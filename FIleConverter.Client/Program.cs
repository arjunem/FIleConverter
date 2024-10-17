namespace FileConverter.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set the folder to monitor
            string folderToWatch = @"D:\Learn\FIleConverter\Files"; // Change to the folder you want to monitor

            // Create a FileSystemWatcher to monitor the directory
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = folderToWatch;

            // Watch for changes in the creation of new files
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime;

            // Filter to only watch for file creation (can be adjusted as needed)
            watcher.Filter = "*.csv*"; // Watch for all file types

            // Add event handler for the Created event (file added)
            watcher.Created += OnNewFileAdded;

            // Begin watching
            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Monitoring folder: {folderToWatch}. Press 'q' to quit.");

            TCPFileClient.ReadHeartBeat();

            // Keep the console app running until the user quits
            while (Console.Read() != 'q') ;
        }

        // Event handler for file created
        private static void OnNewFileAdded(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(100);

            // Trigger your console application here
            TCPFileClient.SendFile(e.FullPath);
            //TriggerConsoleApp(e.FullPath);
        }
    }
}
