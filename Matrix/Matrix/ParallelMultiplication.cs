using System;
using System.Threading;

namespace Matrix
{
    /// <summary>
    /// Implementation of parallel matrix multiplication
    /// </summary>
    public static class ParallelMultiplication
    {
        /// <summary>
        /// Multiplies matrices with parallel realisation
        /// </summary>
        /// <param name="firstMatrix">First matrix in multiplication</param>
        /// <param name="secondMatrix">Second matrix in multiplication</param>
        /// <returns>Returns matrix after multiplication</returns>
        public static int[,] ParallelMultiply(this int[,] firstMatrix, int[,] secondMatrix)
        {
            var rowsCountFirst = firstMatrix.GetLength(0);
            var colsCountFirst = firstMatrix.GetLength(1);
            var rowsCountSecond = secondMatrix.GetLength(0);
            var colsCountSecond = secondMatrix.GetLength(1);
            if (colsCountFirst != rowsCountSecond)
            {
                throw new InvalidSizesMultiplicationException("Invalid matrices sizes");
            }

            var threads = new Thread[Math.Min(Environment.ProcessorCount, rowsCountFirst)];
            var chunkSize = rowsCountFirst / threads.Length + 1;
            var resultMatrix = new int[rowsCountFirst, colsCountSecond];
            for (var i = 0; i < threads.Length; i++)
            {
                var local = i;
                threads[i] = new Thread(() =>
                {
                    for (var row = local * chunkSize; row < Math.Min((local + 1) * chunkSize, rowsCountFirst); row++)
                    {
                        for (var column = 0; column < colsCountSecond; column++)
                        {
                            for (var j = 0; j < colsCountFirst; j++)
                            {
                                resultMatrix[row, column] += firstMatrix[row, j] * secondMatrix[j, column];
                            }
                        }
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return resultMatrix;
        }
    }
}