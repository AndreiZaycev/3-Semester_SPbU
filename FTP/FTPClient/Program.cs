using System.Net;

namespace FTP;

public static class Program
{
    private static async Task Main(string[] args)
    {
        void ShowHelp()
        {
            Console.WriteLine("Input IP address and port to start the client");
            Console.WriteLine("Commands: ");
            Console.WriteLine("--list {path} - returns list of files in the directory");
            Console.WriteLine("--get {filePath} {pathToDownloadFile} - downloads the specified file");
        }

        if (args.Length != 4 && args.Length != 5)
        {
            ShowHelp();
            return;
        }

        if (!IPAddress.TryParse(args[2], out _))
        {
            ShowHelp();
            Console.WriteLine("Incorrect IP");
            return;
        }

        if (!int.TryParse(args[3], out var port) || port is < 0 or > 65535)
        {
            ShowHelp();
            Console.WriteLine("Incorrect port");
            return;
        }

        var client = new Client(args[2], Convert.ToInt32(args[3]));

        switch (args[0])
        {
            case "--list":
                var response = await client.List(args[1]);
                Console.WriteLine(response.Count);
                foreach (var (path, isDirectory) in response)
                {
                    Console.WriteLine($"{path} {isDirectory}");
                }

                return;
            case "--get":
                if (args.Length != 5)
                {
                    ShowHelp();
                }

                var size = await client.Get(args[1], args[4]);
                Console.WriteLine(size);
                return;
            default:
                Console.WriteLine("Incorrect command");
                return;
        }
    }
}