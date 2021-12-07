using System;
using System.Diagnostics;
using System.IO;

namespace MD5
{
    public static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(Measurements(args[0]));                
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static string Measurements(string inputPath)
        {
            var md5Hash = System.Security.Cryptography.MD5.Create();
            var firstTimer = new Stopwatch();
            var secondTimer = new Stopwatch();
            var calculator = new Hasher(md5Hash);
            firstTimer.Start();
            calculator.CalculatePath(inputPath, false);
            firstTimer.Stop();
            var firstTime = firstTimer.ElapsedMilliseconds;
            secondTimer.Start();
            calculator.CalculatePath(inputPath, true);
            secondTimer.Stop();
            var secondTime = secondTimer.ElapsedMilliseconds;
            return $"последовательная программа {firstTime}\n параллельная программа {secondTime}";
        }
    }
}