namespace FIleConverter.Server
{
    public class AppConfig
    {
        public ApplicationSettings ApplicationSettings { get; set; }
    }

    public class ApplicationSettings
    {
        public string DownloadFolder { get; set; }
        public string ServerIP { get; set; }
        public int Port { get; set; }
        public int HeartBeatTimeout { get; set; }
    }
}
