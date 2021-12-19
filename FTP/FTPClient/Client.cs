using System.Net.Sockets;
using System.Text;

namespace FTP;

/// <summary>
/// Implementation of the client
/// </summary>
public class Client
{
    private readonly string _ip;
    private readonly int _port;

    /// <summary>
    /// Creates client
    /// </summary>
    /// <param name="ip">Client's ip</param>
    /// <param name="port">Client's port</param>
    public Client(string ip, int port)
    {
        _ip = ip;
        _port = port;
    }

    /// <summary>
    /// Gets all files and directories in the specified directory 
    /// </summary>
    /// <param name="path">Path of directory</param>
    /// <returns>List of files and directories</returns>
    public async Task<List<(string, bool)>> List(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(_ip, _port);
        var stream = client.GetStream();
        await using var writer = new StreamWriter(stream) {AutoFlush = true};
        await writer.WriteLineAsync($"1 {path}");
        using var reader = new StreamReader(stream);
        var data = await reader.ReadToEndAsync();
        if (data == "-1")
        {
            throw new DirectoryNotFoundException();
        }
        var tokens = data.Split(' ');
        var result = new List<(string, bool)>();
        for (var i = 1; i < tokens.Length; i += 2)
        {
            result.Add((tokens[i], Convert.ToBoolean(tokens[i + 1])));
        }

        return result;
    }
        
    /// <summary>
    ///  Downloads file
    /// </summary>
    /// <param name="pathToDownload">Path to download</param>
    /// <param name="pathToSave">Path to save</param>
    /// <returns>Returns size of file</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<int> Get(string pathToDownload, string pathToSave)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(_ip, _port);
        await using var stream = client.GetStream();
        await using var writer = new StreamWriter(stream) {AutoFlush = true};
        using var reader = new StreamReader(stream);
        await writer.WriteLineAsync($"2 {pathToDownload}");
        var size = new StringBuilder();
        while (reader.Peek() != ' ')
        {
            if (reader.Peek() == '-')
            {
                throw new FileNotFoundException();
            }

            size.Append((char)reader.Read());
        }

        reader.Read();
        await using var fileStream = File.Create(pathToSave);
        await stream.CopyToAsync(fileStream);
        return Convert.ToInt32(size.ToString());
    }
}