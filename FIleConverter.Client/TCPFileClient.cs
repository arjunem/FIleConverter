using System.Net.Sockets;
using System.Text;

namespace FileConverter.Client
{
    public class TCPFileClient
    {
        private const string serverIP = "127.0.0.1"; // Replace with the IP address of the Linux VM
        private const int serverPort = 1234; // Port of the TCP server
        private static bool isServerRunning = true;
        private const int heartbeatTimeout = 25000;

        public static void ReadHeartBeat(string? guestIP = null, int? port = null, int? timeout = null)
        {
            try
            {
                // Connect to the server
                TcpClient client = new TcpClient(guestIP ?? serverIP, port ?? serverPort);
                NetworkStream stream = client.GetStream();
                stream.ReadTimeout = 10000; // Set a 10-second timeout for reading
                Console.WriteLine("Connected to server.");

                // Start a thread to receive heartbeat from the server
                Thread receiveThread = new Thread(() => ReceiveHeartbeat(stream));
                receiveThread.Start();

                // Keep the client running to simulate monitoring
                while (true)
                {
                    Thread.Sleep(10000); // Simulate doing other things
                }
                // Close the connection
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        public static bool SendFile(string fullpath = null, string? guestIP = null, int? port = null)
        {
            bool isSend = false;
            try
            {
                if (isServerRunning)
                {
                    Console.WriteLine($"New file detected: {fullpath}");

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
                    isSend = true;
                }
            }
            catch(ArgumentNullException ex)
            {
                throw ex;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
            catch (SocketException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return isSend;
        }

        private static void ReceiveHeartbeat(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (stream.CanRead)
            {
                try
                {
                    // Start reading from the stream
                    bytesRead = ReadWithTimeout(stream, buffer, heartbeatTimeout); // 25 seconds timeout

                    // Read data from the server
                    bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        // Convert the received byte array to a string
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        // Check if the message is a heartbeat
                        if (message.Trim() == "HEARTBEAT")
                        {
                            //Console.WriteLine("Received heartbeat from server.");
                        }
                        else
                        {
                            Console.WriteLine($"Received unexpected message: {message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No data received from the server.");
                    }
                }
                catch (IOException ex)
                {
                    // If the stream cannot be read from, it might be because the server disconnected
                    Console.WriteLine($"Connection lost. Error receiving data: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    // Handle other potential exceptions
                    Console.WriteLine($"Error: {ex.Message}");
                    stream.Close();
                    stream.Socket.Close();
                    isServerRunning = false;
                    Console.WriteLine("Press 'q' to quit.");
                    if (Console.ReadLine() == "q")
                    {
                        Environment.Exit(0);
                    }
                    break;
                }
            }

            // Close the stream if it's no longer readable
            stream.Close();
            Console.WriteLine("Stream closed, stopping heartbeat listener.");
        }
        private static int ReadWithTimeout(NetworkStream stream, byte[] buffer, int timeoutMilliseconds)
        {
            // Start a timer for the timeout
            DateTime startTime = DateTime.Now;
            int totalBytesRead = 0;
            while (true)
            {
                // Check if the timeout has elapsed
                if ((DateTime.Now - startTime).TotalMilliseconds > timeoutMilliseconds)
                {
                    Console.WriteLine("Read operation timed out.");
                    throw new TimeoutException("Please check the server is running correctly!!");
                }

                // Check if data is available to read
                if (stream.DataAvailable)
                {
                    int bytesRead = stream.Read(buffer, totalBytesRead, buffer.Length - totalBytesRead);
                    totalBytesRead += bytesRead;

                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Server has closed the connection.");
                        break; // Server has closed the connection
                    }

                    // If we've read data, break the loop
                    if (totalBytesRead > 0)
                    {
                        return totalBytesRead; // Return the number of bytes read
                    }
                }

                // Sleep briefly to avoid busy waiting
                Thread.Sleep(100);
            }

            return totalBytesRead;
        }
    }
}
