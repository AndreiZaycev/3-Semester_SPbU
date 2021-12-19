using System.Net;

namespace FTPServer;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Write IP address and port to start the server");
            return;
        }

        if (!IPAddress.TryParse(args[0], out _))
        {
            Console.WriteLine("Incorrect IP");
            return;
        }

        if (!int.TryParse(args[1], out var port) || port is < 0 or > 65535)
        {
            Console.WriteLine("Incorrect port");
            return;
        }

        var server = new Server(IPAddress.Parse(args[0]), Convert.ToInt32(args[1]));
        await server.Start();
    }
}