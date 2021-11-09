using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Test
{
    class Server
    {
        private readonly TcpListener _listener;
        private readonly TcpClient _client = new();

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task Run()
        {
            _listener.Start();
            Console.WriteLine($"Listening...");
            while (true)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"Client connected!");
                ReceiveMessage(tcpClient.GetStream());
                SendMessage(tcpClient.GetStream());
            }
        }

        private void Stop()
        {
            _client.Close();
            _listener.Stop();
            Environment.Exit(1);
        }

        private void CheckStop(string data)
        {
            if (data == "exit")
            {
                Stop();
            }
        }

        private void SendMessage(Stream stream)
        {
            Task.Run(async () =>
            {
                var writer = new StreamWriter(stream) {AutoFlush = true};
                while (true)
                {
                    Console.WriteLine("-------------------");
                    var data = Console.ReadLine();
                    await writer.WriteLineAsync(data);
                    CheckStop(data);
                }
            });
        }

        private void ReceiveMessage(Stream stream)
        {
            Task.Run(async () =>
            {
                var reader = new StreamReader(stream);
                while (true)
                {
                    Console.WriteLine("-------------------");
                    var data = await reader.ReadLineAsync();
                    Console.WriteLine(data);
                    CheckStop(data);
                }
            });
        }
    }
}
