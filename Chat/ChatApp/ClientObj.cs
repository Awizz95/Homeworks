using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServerApp
{
    public class ClientObj
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public StreamWriter Writer { get; }
        public StreamReader Reader { get; }

        Server server;
        TcpClient client;

        public ClientObj(TcpClient tcpClient, Server server)
        {
            client = tcpClient;
            this.server = server;
            var stream = client.GetStream();

            Reader = new StreamReader(stream);
            Writer = new StreamWriter(stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                string? userName = await Reader.ReadLineAsync();
                string? message = $"{userName} вошел в чат!";

                await server.BroadcastMessageAsync(message, Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = string.Empty;
                        message = await Reader.ReadLineAsync();
                        if (message == null) continue;
                        message = $"Новое сообщение от пользователя {userName}: {message}";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
                    }
                    catch
                    {
                        message = $"{userName} покинул чат";
                        Console.WriteLine(message);
                        await server.BroadcastMessageAsync(message, Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(Id);
            }
        }

        public void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
