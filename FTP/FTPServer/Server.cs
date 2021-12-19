using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FTPServer;

/// <summary>
/// Implementation of the server
/// </summary>
public class Server
{
    private readonly IPAddress _ip;
    private readonly int _port;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly List<Task> _clients;

    /// <summary>
    /// Creates new server
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public Server(IPAddress ip, int port)
    {
        _ip = ip;
        _port = port;
        _cancellationTokenSource = new CancellationTokenSource();
        _clients = new List<Task>();
    }

    private static async Task List(TextWriter writer, string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            await writer.WriteAsync("-1");
            return;
        }

        var files = Directory.GetFiles(directoryPath);
        var directories = Directory.GetDirectories(directoryPath);
        var size = files.Length + directories.Length;
        var result = new StringBuilder(size);
        foreach (var directory in directories)
        {
            result.Append($" {directory} true");
        }

        foreach (var file in files)
        {
            result.Append($" {file} false");
        }

        await writer.WriteAsync(result);
    }

    private static async Task Get(StreamWriter writer, string path)
    {
        if (!File.Exists(path))
        {
            await writer.WriteAsync("-1");
            return;
        }
            
        await writer.WriteAsync($"{new FileInfo(path).Length} ");
        await File.OpenRead(path).CopyToAsync(writer.BaseStream);
    }
        
    private static async Task Work(Socket socket)
    {
        using (socket)
        {
            await using var stream = new NetworkStream(socket);
            using var reader = new StreamReader(stream);
            var arguments = (await reader.ReadLineAsync())?.Split(' ');
            await using var writer = new StreamWriter(stream) {AutoFlush = true};
            if (arguments != null)
            {
                if (arguments.Length != 2)
                {
                    await writer.WriteLineAsync("Number of arguments should be 2");
                    return;
                }
                    
                switch (arguments[0])
                {
                    case "1":
                        await List(writer, arguments[1]);
                        return;
                    case "2":
                        await Get(writer, arguments[1]);
                        return;
                    default:
                        await writer.WriteAsync("Incorrect arguments");
                        return;
                }
            }
        }
    }

    /// <summary>
    /// Stops the server
    /// </summary>
    public void Stop() => _cancellationTokenSource.Cancel();

    /// <summary>
    /// Starts the server 
    /// </summary>
    public async Task Start()
    {
        var listener = new TcpListener(_ip, _port);
        listener.Start();
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync();
            var client = Task.Run(() => Work(socket));
            _clients.Add(client);
        }

        Task.WaitAll(_clients.ToArray());
        listener.Stop();
    }
}