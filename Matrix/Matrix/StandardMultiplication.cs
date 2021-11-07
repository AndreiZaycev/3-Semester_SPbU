namespace Matrix
{
    /// <summary>
    /// Implementation of standard matrix multiplication
    /// </summary>
    public static class StandardMultiplication
    {
        /// <summary>
        /// Multiplies matrices with naive realisation
        /// </summary>
        /// <param name="firstMatrix">First matrix in multiplication</param>
        /// <param name="secondMatrix">Second matrix in multiplication</param>
        /// <returns>Returns matrix after multiplication</returns>
        public static int[,] StandardMultiply(this int[,] firstMatrix, int[,] secondMatrix)
        {
            var rowsCountFirst = firstMatrix.GetLength(0);
            var colsCountFirst = firstMatrix.GetLength(1);
            var rowsCountSecond = secondMatrix.GetLength(0);
            var colsCountSecond = secondMatrix.GetLength(1);
            if (colsCountFirst != rowsCountSecond)
            {
                throw new InvalidSizesMultiplicationException("Invalid matrices sizes");
            }

            var resultMatrix = new int[rowsCountFirst, colsCountSecond];
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
}