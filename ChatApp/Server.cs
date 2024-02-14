using Microsoft.VisualBasic;
using System.Net;
using System.Net.Sockets;

namespace ChatServerApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var server = new Server();
            await server.Run();
        }
    }

    public class Server
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 8888);
        List<TcpClient> users = new List<TcpClient>();

        public async Task Run()
        {
            try
            {
                listener.Start();

                await Console.Out.WriteLineAsync("Сервер запущен!");

                while (true)
                {
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    users.Add(tcpClient);

                    Console.WriteLine("Пользователь успешно подключен!");

                    await Task.Run(() => ProcessClient(tcpClient));
                }
            }
            catch { }
            finally { }
        }

        public async Task ProcessClient(TcpClient client)
        {
            var reader = new StreamReader(client.GetStream());

            while (client.Connected)
            {
                string? msg = await reader.ReadLineAsync();

                if (msg != null)
                {
                    foreach (var user in users)
                    {
                        if (user != client)
                        {
                            using (var writer = new StreamWriter(user.GetStream()))
                            {
                                await writer.WriteLineAsync(msg);
                            };
                        }
                    }
                }

                reader.Close();
            }
        }
    }
}