using Microsoft.VisualBasic;
using System.Net;
using System.Net.Sockets;

public class Client
{
    static async Task Main(string[] args)
    {
        TcpClient tcpClient = new TcpClient();

        Thread.Sleep(2000);

        tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 8888);

        Console.WriteLine("Вы успешно подключены!");
        Console.WriteLine("Вы можете отправлять сообщения на сервер. Для выхода из чата нажмите \"q\"");

        string? outMsg = string.Empty;
        var sWriter = new StreamWriter(tcpClient.GetStream());
        var sReader = new StreamReader(tcpClient.GetStream());

        while (true)
        {
            Console.Write("Введите сообщение: ");
            await Task.Run(() => outMsg = Console.ReadLine());

            if (outMsg == "q") break;
            else if (outMsg != null)
            {
                await sWriter.WriteLineAsync(outMsg);
                sWriter.Flush();
                Console.WriteLine("Ваше сообщение отправлено!");
            }

            string? inMsg = await sReader.ReadLineAsync();

            if (inMsg != null) Console.WriteLine("Сторонний пользователь написал: " + inMsg);

        }
        Console.WriteLine("Приложение закрывается");
        Console.ReadLine();

    }
}