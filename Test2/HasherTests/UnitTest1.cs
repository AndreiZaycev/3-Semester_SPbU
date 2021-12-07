using System.IO;
using MD5;
using NUnit.Framework;

public class Tests
{

    [Test]
    public void Test1()
    {
        var inputPath = $"{Directory.GetCurrentDirectory()}\\abacaba";
        var md5Hash = System.Security.Cryptography.MD5.Create();
        var calculator = new Hasher(md5Hash);
        var first = calculator.CalculatePath(inputPath, false);
        var second = calculator.CalculatePath(inputPath, true);
        Assert.AreEqual(first, second);
    }
}