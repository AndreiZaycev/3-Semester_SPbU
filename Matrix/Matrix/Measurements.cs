using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Matrix
{
    class Measurements
    {
        static void Main()
        {
            static void Measure(int iterations, int size) 
            {
                var resultsStandard = new List<long>();
                var resultsParallel = new List<long>();
                var timer = new Stopwatch();
                for (var i = 0; i < iterations; i++)
                {
                    var matrixA = Generator.Generate(size, size);
                    var matrixB = Generator.Generate(size, size);

                    timer.Restart();
                    matrixA.StandardMultiply(matrixB);
                    timer.Stop();
                    resultsStandard.Add(timer.ElapsedMilliseconds);
                    timer.Restart();
                    matrixA.ParallelMultiply(matrixB);
                    timer.Stop();
                    resultsParallel.Add(timer.ElapsedMilliseconds);
                }

                static (double, double) FindAverageAndDeviation(IReadOnlyCollection<long> results)
                {
                    var average = results.Average();
                    var variance = results.Select(x =>
                        Math.Pow(x - average, 2)).Average();
                    var deviation = Math.Sqrt(variance);
                    return (average, deviation);
                }

                var (averageStandard, deviationStandard) =
                    FindAverageAndDeviation(resultsStandard);
                var (averageParallel, deviationParallel) =
                    FindAverageAndDeviation(resultsParallel);
                Console.WriteLine("Average time for standard multiplication: " + averageStandard + " ms");
                Console.WriteLine("Deviation time for standard multiplication: " + deviationStandard + " ms");
                Console.WriteLine("Average time for parallel multiplication: " + averageParallel + " ms");
                Console.WriteLine("Deviation time for parallel multiplication: " + deviationParallel + " ms");
            }

            Measure(50, 128);
            Measure(50, 256);
            Measure(50, 512);
            Measure(50, 1024);
        }
        /*
        128 x 128:
            Average time for standard multiplication: 22,5 ms
            Deviation time for standard multiplication: 2,2203603311174516 ms
            Average time for parallel multiplication: 9,96 ms
            Deviation time for parallel multiplication: 1,5994999218505759 ms
        256x256:
            Average time for standard multiplication: 199,52 ms
            Deviation time for standard multiplication: 16,582207331956745 ms
            Average time for parallel multiplication: 71,1 ms
            Deviation time for parallel multiplication: 9,148223871331528 ms
        512 x 512:   
            Average time for standard multiplication: 1936 ms
            Deviation time for standard multiplication: 267,0062171560805 ms
            Average time for parallel multiplication: 617,74 ms
            Deviation time for parallel multiplication: 41,91840168708726 ms
        1024 x 1024:
            Average time for standard multiplication: 27770,5 ms
            Deviation time for standard multiplication: 2074,3805460908084 ms
            Average time for parallel multiplication: 7371,22 ms
            Deviation time for parallel multiplication: 754,4317938687366 ms
        */
    }
}