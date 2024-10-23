namespace FIleConverter.Client
{
    public class AppConfig
    {
        public ApplicationSettings ApplicationSettings { get; set; }
    }

    public class ApplicationSettings
    {
        public string FolderToWatch { get; set; }
        public string ServerIP { get; set; }
        public int Port { get; set; }
    }
}
