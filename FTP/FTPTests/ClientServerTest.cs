using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FTP;
using FTPServer;

using NUnit.Framework;

namespace FTPTests;

public class Tests
{
    private readonly string _path = Path.Join("..", "..", "..", "..", "FTPTests", "TestDirectory");
    private const string Ip = "127.0.0.1";
    private const int Port = 8888;
    private readonly Client _client = new(Ip, Port);
    private readonly Server _server = new(IPAddress.Parse(Ip), Port);

    [SetUp]
    public void Setup()
    {
        _server.Start();
    }

    [Test]
    public async Task ListShouldReturnRightSizeAndItems()
    {
        var actual = await _client.List(_path);
        Assert.AreEqual(2, actual.Count);
        var expected = new[]
        {
            ($"{_path}//Test", true),
            ($"{_path}//test1.txt", false)
        };

        for (var i = 0; i < actual.Count; i++)
        {
            Assert.AreEqual(expected[i], actual[i]);
        }

    }

    [Test]
    public async Task GetShouldReturnRightSizeAndDownloadFile()
    {
        var filePath = Path.Join(_path, "test1.txt");
        var pathToDownload = Path.Join(_path, "test3.txt");
        var actual = await _client.Get(filePath, pathToDownload);
        FileAssert.AreEqual(filePath, pathToDownload);
        File.Delete(pathToDownload);
        Assert.AreEqual(13, actual);
    }

    [Test]
    public void ExceptionShouldRaiseWhenFileDoesNotExist()
    {
        var filePath = Path.Join(_path, "bebra.txt");
        var pathToDownload = Path.Join(_path, "bebrix.txt");
        Assert.Throws<AggregateException>(() => _client.Get(filePath, pathToDownload).Wait());
    }

    [Test]
    public void ExceptionShouldRaiseWhenDirectoryDoesNotExist()
    {
        var path = Path.Join(_path, "lol");
        Assert.Throws<AggregateException>(() => _client.List(path).Wait());
    }
}
