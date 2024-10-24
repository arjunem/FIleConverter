using System.Net.Sockets;

namespace FileConverter.Client.Test
{
    public class FileConverterClientTest
    {
        [Fact]
        public void SendFile_ShouldThrowEmptyFilePathException()
        {
            // Arrange

            // Act + Assert
            var exception = Assert.Throws<ArgumentNullException>(() => TCPFileClient.SendFile());

            //Compare
            Assert.Equal("Value cannot be null. (Parameter 's')", exception.Message);

        }

        [Fact]
        public void SendFile_ShouldThrowFileNotFoundException()
        {
            // Arrange
            string file = @"D:\Learn\FIleConverter\Files\currency_not_found_file.csv";

            // Act + Assert
            var exception = Assert.Throws<FileNotFoundException>(() => TCPFileClient.SendFile(file));

        }

        [Fact]
        public void SendFile_ShouldThrowSocketException()
        {
            // Arrange
            string filePath = @"D:\Learn\FIleConverter\Files\currency.csv";
            string falseIp = "127.0.0.1";
            int falsePort = 0;

            // Act + Assert
            var exception = Assert.Throws<SocketException>(() => TCPFileClient.SendFile(filePath,falseIp,falsePort));

        }

        [Fact]
        public void SendFile_ShouldSendFileToServerSuccessfully()
        {
            // Arrange
            string file = @"D:\Learn\FIleConverter\Files\currency.csv";

            // Act
            var isSend = TCPFileClient.SendFile(file);

            // Assert
            Assert.True(isSend);

        }

    }
}