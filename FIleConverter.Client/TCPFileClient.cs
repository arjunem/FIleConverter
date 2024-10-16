using System.Net.Sockets;
using System.Text;

namespace FileConverter.Client
{
    public class TCPFileClient
    {
        private const string serverIP = "127.0.0.1"; // Replace with the IP address of the Linux VM
        private const int serverPort = 1234; // Port of the TCP server

        public static void SendFile(string fullpath = null, string? guestIP = null, int? port = null)
        {
            try
            {
                // Get the file name
                string fileName = Path.GetFileName(fullpath);

                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

                // Open the file and read the data into a byte array
                byte[] fileData = File.ReadAllBytes(fullpath);

                // Create a TcpClient and connect to the server
                TcpClient client = new TcpClient(guestIP ?? serverIP, port ?? serverPort);
                NetworkStream stream = client.GetStream();

                // Send the file name size first (4 bytes)
                byte[] fileNameSize = BitConverter.GetBytes(fileNameBytes.Length);
                stream.Write(fileNameSize, 0, fileNameSize.Length);

                // Send the file name
                stream.Write(fileNameBytes, 0, fileNameBytes.Length);

                // Send the file size (4 bytes)
                byte[] fileSize = BitConverter.GetBytes(fileData.Length);
                stream.Write(fileSize, 0, fileSize.Length);

                // Send the file data
                stream.Write(fileData, 0, fileData.Length);
                Console.WriteLine("File sent successfully!");

                // Close the stream and the client
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
