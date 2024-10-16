﻿using System.Net;
using System.Net.Sockets;
using System.Text;
namespace FileConverter.Server
{
    class TCPFileServer
    {
        private const int port = 1235; // Port to listen on
        private const string saveFilePath = "/home/amukundane/Documents/received_file.txt"; // Update to your desired path
        private static bool keepRunning = true;
        public static void RecieveFile()
        {
            try
            {

                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                listener.Start();

                Console.WriteLine("Server is listening on port " + port + "...");

                // Start a task to monitor user input for the "q" command
                Task.Run(() => MonitorExitCommand());

                while (keepRunning)
                {
                    if (listener.Pending()) // Check if any client is trying to connect
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        Console.WriteLine("Client connected.");

                        // Handle the client connection in a new task
                        Task.Run(() => HandleClient(client));
                    }
                }
                listener.Stop();
                Console.WriteLine("Server stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                // Read the file name size (first 4 bytes)
                byte[] fileNameSizeBytes = new byte[4];
                stream.Read(fileNameSizeBytes, 0, fileNameSizeBytes.Length);
                int fileNameSize = BitConverter.ToInt32(fileNameSizeBytes, 0);

                // Read the file name
                byte[] fileNameBytes = new byte[fileNameSize];
                stream.Read(fileNameBytes, 0, fileNameBytes.Length);
                string fileName = Encoding.UTF8.GetString(fileNameBytes);

                Console.WriteLine("Receiving file: " + fileName);

                // Read the file size (next 4 bytes)
                byte[] fileSizeBytes = new byte[4];
                stream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
                int fileSize = BitConverter.ToInt32(fileSizeBytes, 0);

                Console.WriteLine("File size: " + fileSize + " bytes");

                // Receive the file data
                byte[] fileData = new byte[fileSize];
                int bytesRead = 0, totalBytesRead = 0;
                while (totalBytesRead < fileSize)
                {
                    bytesRead = stream.Read(fileData, totalBytesRead, fileSize - totalBytesRead);
                    totalBytesRead += bytesRead;
                }

                // Save the file with the received file name
                string savePath = Path.Combine("/home/amukundane/Documents/DownloadedTSVs/", fileName);  // Change the directory as needed
                File.WriteAllBytes(savePath, fileData);
                Console.WriteLine("File received and saved to: " + savePath);

                // Close the connections
                stream.Close();
                client.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
        }
        static void MonitorExitCommand()
        {
            // Keep checking for user input
            while (keepRunning)
            {
                string input = Console.ReadLine();
                if (input?.ToLower() == "q")
                {
                    keepRunning = false;
                    Console.WriteLine("Exiting server...");
                }
            }
        }
    }
}