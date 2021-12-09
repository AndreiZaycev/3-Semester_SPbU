namespace Matrix;

/// <summary>
/// Implementation of standard matrix multiplication
/// </summary>
public class StandardMultiplication : IMultiplication
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

        var resultMatrix = new Matrix(rowsCountFirst, colsCountSecond);
        for (var row = 0; row < rowsCountFirst; row++)
        {
            for (var column = 0; column < colsCountSecond; column++)
            {
                for (var i = 0; i < colsCountFirst; i++)
                {
                    resultMatrix[row, column] += firstMatrix[row, i] * secondMatrix[i, column];
                }
            }
        }

        return resultMatrix;
    }
}