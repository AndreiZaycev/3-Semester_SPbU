using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Test
{
    class Client
    {
        private readonly TcpClient _client;
        private Stream _stream;

        public Client(string hostname, int port)
        {
            _client = new TcpClient(hostname, port);
        }

        private void Stop()
        {
            _stream.Close();
            _client.Close();
            Environment.Exit(1);
        }

        public async Task Run()
        {
            _stream = _client.GetStream();
            Console.WriteLine("Client is ready to chat!");

            ReceiveMessage();
            await SendMessage();
        }

        private void CheckStop(string data)
        {
            if (data == "exit")
            {
                Stop();
            }
        }

        private void ReceiveMessage()
        {
            Task.Run(async () =>
            {
                var reader = new StreamReader(_stream);
                while (true)
                {
                    Console.WriteLine("-------------------");
                    var data = await reader.ReadLineAsync();
                    Console.WriteLine(data);
                    CheckStop(data);
                }
            });
        }

        private async Task SendMessage()
        {
            var writer = new StreamWriter(_stream) {AutoFlush = true};
            while (true)
            {
                Console.WriteLine("-------------------");
                var data = Console.ReadLine();
                await writer.WriteLineAsync(data);
                CheckStop(data);
            }
        }
    }
}
