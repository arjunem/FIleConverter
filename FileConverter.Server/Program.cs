namespace FileConverter.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TCPFileServer.RecieveFile();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }
    }
}
