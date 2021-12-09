using System;
using System.Threading;

namespace Matrix;

/// <summary>
/// Implementation of parallel matrix multiplication
/// </summary>
public class ParallelMultiplication : IMultiplication
{
    /// <inheritdoc/>
    public Matrix Multiply(Matrix firstMatrix, Matrix secondMatrix)
    {
        var rowsCountFirst = firstMatrix.Rows;
        var colsCountFirst = firstMatrix.Cols;
        var rowsCountSecond = secondMatrix.Rows;
        var colsCountSecond = secondMatrix.Cols;
        if (colsCountFirst != rowsCountSecond)
        {
            throw new InvalidSizesMultiplicationException("Invalid matrices sizes");
        }

        var threads = new Thread[Math.Min(Environment.ProcessorCount, rowsCountFirst)];
        var chunkSize = rowsCountFirst / threads.Length + 1;
        var resultMatrix = new Matrix(rowsCountFirst, colsCountSecond);
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