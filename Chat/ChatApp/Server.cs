﻿using Microsoft.VisualBasic;
using System.Net;
using System.Net.Sockets;

namespace ChatServerApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            var server = new Server(token);

            Task.Run(() =>
            {
                Thread.Sleep(25000);
                source.Cancel();
            });

            await server.Run();
        }
    }

    public class Server
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 8888);
        List<ClientObj> users = new List<ClientObj>();
        CancellationToken token;

        public Server(CancellationToken token)
        {
            this.token = token;
        }

        public async Task Run()
        {
            try
            {
                listener.Start();

                Console.WriteLine("Сервер запущен!");
                await Console.Out.WriteLineAsync("Отправьте \"q\" или \"Q\", чтобы выключить сервер!");

                Task.Run(() =>
                {
                    while (true)
                    {
                        string? q = default;
                        q = Console.ReadLine();
                        if (q == "q" || q == "Q")
                            Disconnect();
                    }
                });

                Task.Run(() =>
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested) Disconnect();
                    }
                });

                while (true)
                {
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    ClientObj client = new ClientObj(tcpClient, this);
                    users.Add(client);
                    Task.Run(client.ProcessAsync);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        public async Task BroadcastMessageAsync(string message, string id)
        {
            foreach (var client in users)
            {
                if (client.Id != id)
                {
                    await client.Writer.WriteLineAsync(message);
                    await client.Writer.FlushAsync();
                }
            }
        }

        public void RemoveConnection(string id)
        {
            ClientObj? client = users.FirstOrDefault(c => c.Id == id);

            if (client != null) users.Remove(client);

            client?.Close();
        }

        public void Disconnect()
        {
            foreach (var client in users)
            {
                client.Close();
            }

            Console.WriteLine("Работа сервера завершена!");
            listener.Stop();
        }
    }
}
