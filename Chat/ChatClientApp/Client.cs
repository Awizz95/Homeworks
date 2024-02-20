using Microsoft.VisualBasic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Client
{
    static async Task Main(string[] args)
    {
        TcpClient tcpClient = new TcpClient();
        Console.Write("Введите ваше имя: ");
        string? username = Console.ReadLine();
        StreamWriter sWriter = null;
        StreamReader sReader = null;

        try
        {
            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 8888);

            Console.WriteLine("Здравствуйте, " + username + "! Вы успешно подключены!");
            Console.WriteLine("Вы можете отправлять сообщения на сервер. Отправьте слово \"Exit\", чтобы отключиться от чата!");

            string? outMsg = string.Empty;
            sWriter = new StreamWriter(tcpClient.GetStream());
            sReader = new StreamReader(tcpClient.GetStream());

            Task.Run(() => ReceiveMessageAsync(sReader));

            await SendMessageAsync(sWriter);

        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        sReader?.Close();
        sWriter?.Close();


        async Task SendMessageAsync(StreamWriter sWriter)
        {
            await sWriter.WriteLineAsync(username);
            await sWriter.FlushAsync();

            while (true)
            {
                Console.WriteLine("Введите сообщение: ");
                string? outMsg = Console.ReadLine();

                if (outMsg == "Exit")
                {
                    sReader?.Close();
                    sWriter?.Close();
                    Console.WriteLine("Вы отключены от чата!");
                    break;
                }

                await sWriter.WriteLineAsync(outMsg);
                await sWriter.FlushAsync();
            }
        }

        async Task ReceiveMessageAsync(StreamReader sReader)
        {
            while (true)
            {
                try
                {
                    string? inMsg = await sReader.ReadLineAsync();

                    if (String.IsNullOrEmpty(inMsg)) continue;

                    Console.WriteLine(inMsg);
                }
                catch
                {
                    break;
                }
            }
        }
    }
}