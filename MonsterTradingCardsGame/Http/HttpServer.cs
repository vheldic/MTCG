using System.Net;
using System.Net.Sockets;
using System.Text;
using MonsterTradingCardsGame.Database;

namespace MonsterTradingCardsGame.Http
{
    public class Programm
    {
        static async Task Main()
        {
            // IP adress and port to listen to
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 10001;

            TcpListener tcpListener = new TcpListener(ipAddress, port);

            // listening for incoming connections
            tcpListener.Start();
            Console.WriteLine($"Server started. Listening on {ipAddress}:{port}");

            Database.Database db = new Database.Database();

            while (true)
            {
                // Wait for a client connection
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                // Handle the client asynchronously
                _ = Task.Run(() => HandleClient(client, tcpListener, db));
            }
        }

        static async Task HandleClient(TcpClient client, TcpListener listener, Database.Database db)
        {
            Console.WriteLine($"Client connected {client.Client.RemoteEndPoint}");

            // Client's network stream
            using (NetworkStream networkStream = client.GetStream())
            {

                try
                {
                    var requestBytes = new byte[1024];
                    int bytesRead = await networkStream.ReadAsync(requestBytes, 0, requestBytes.Length);
                    string request = Encoding.UTF8.GetString(requestBytes, 0, bytesRead);
                    Console.WriteLine($"Received request from {client.Client.RemoteEndPoint}:\r\n{request}\r\n");

                    HttpRequest requestHandler = new HttpRequest(listener, request, db);
                    Console.WriteLine(requestHandler.Response);

                    byte[] responseData = Encoding.UTF8.GetBytes(requestHandler.Response);
                    await networkStream.WriteAsync(responseData, 0, responseData.Length);
                    await networkStream.FlushAsync();
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    // Close client connection
                    Console.WriteLine($"Client disconnected: {client.Client.RemoteEndPoint}\r\n");
                    client.Close();
                }
            }
        }
    }
}