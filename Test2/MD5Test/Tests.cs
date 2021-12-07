using System;
using NUnit.Framework;

namespace MD5Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestThatTheTwoProgramsAreWorkingCorrectly()
        {
            var md5Hash = System.Security.Cryptography.MD5.Create();
            var calculator = new Hasher(md5Hash);
            firstTimer.Start();
            calculator.CalculatePath(inputPath, false);
            firstTimer.Stop();
            var firstTime = firstTimer.ElapsedMilliseconds;
            secondTimer.Start();
            calculator.CalculatePath(inputPath, true);
            secondTimer.Stop();
        }
    }
}