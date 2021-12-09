#nullable enable
using System;

namespace Matrix;

/// <summary>
/// Implementation of matrix 
/// </summary>
public class Matrix
{
    public int Rows { get; }
    public int Cols { get; }
    private readonly int[,] _matrix;

    public Matrix(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        _matrix = new int[rows, cols];
    }

    public Matrix(int[,] matrix)
    {
        Rows = matrix.GetLength(0);
        Cols = matrix.GetLength(1);
        _matrix = matrix;
    }

    /// <summary>
    /// Multiplies matrices
    /// </summary>
    /// <param name="matrix">Second matrix in multiplication</param>
    /// <param name="isParallel">True if you need to multiply matrices in parallel mode. Set False by default</param>
    /// <returns>Returns matrix after multiplication</returns>
    public Matrix Multiply(Matrix matrix, bool isParallel = false)
    {
        if (isParallel)
        {
            return new ParallelMultiplication().Multiply(this, matrix);
        }

        return new StandardMultiplication().Multiply(this, matrix);
    }

    public int this[int i, int j]
    {
        get => _matrix[i, j];
        set => _matrix[i, j] = value;
    }

    /// <summary>
    /// Generates random matrix
    /// </summary>
    /// <param name="rows">Numbers of rows in generated matrix.</param>
    /// <param name="cols">Numbers of cols in generated matrix.</param>
    /// <returns>Returns generated matrix</returns>
    public static Matrix Generate(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        var random = new Random();
        var resultMatrix = new Matrix(rows, cols);
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                resultMatrix[row, col] = random.Next();
            }
        }

        return resultMatrix;
    }

    public override bool Equals(object? obj)
    {
        if (obj?.GetType() != GetType())
        {
            return false;
        }

        var right = (Matrix) obj;

        if (Cols != right.Cols || Rows != right.Rows)
        {
            return false;
        }

        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Cols; j++)
            {
                if (this[i, j] != right[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(_matrix, Cols, Rows);
    }

    /// <summary>
    /// Prints matrix in file
    /// </summary>
    /// <param name="matrix">Matrix to print</param>
    /// <param name="path">Path of file to print matrix</param>
    public static void Print(Matrix matrix, string path)
    {
        var rowLine = "";
        for (var row = 0; row < matrix.Rows; row++)
        {
            for (var column = 0; column < matrix.Cols; column++)
            {
                rowLine += $"{matrix[row, column]} ";
            }

            rowLine = $"{rowLine.TrimEnd()}\n";
        }

        System.IO.File.WriteAllText(path, rowLine);
    }

    /// <summary>
    /// Reads matrix from file
    /// </summary>
    /// <param name="path">Path of file to read</param>
    /// <returns>Returns read matrix</returns>
    public static Matrix Read(string path)
    {
        var allLines = System.IO.File.ReadAllLines(path);
        var firstDimension = allLines[0].Split(" ").Length;
        var secondDimension = allLines.Length;
        var resultMatrix = new Matrix(firstDimension, secondDimension);
        for (var rows = 0; rows < firstDimension; rows++)
        {
            var line = allLines[rows].Split(" ");
            for (var columns = 0; columns < secondDimension; columns++)
            {
                resultMatrix[rows, columns] = System.Convert.ToInt32(line[columns]);
            }
        }

        return resultMatrix;
    }
}