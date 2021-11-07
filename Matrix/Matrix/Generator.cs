namespace Matrix
{
    /// <summary>
    /// Implementation of matrix generator
    /// </summary>
    public class Generator
    {
        /// <summary>
        /// Generates random matrix
        /// </summary>
        /// <param name="rows">Numbers of rows in generated matrix.</param>
        /// <param name="columns">Numbers of cols in generated matrix.</param>
        /// <returns>Returns generated matrix</returns>
        /// <exception cref="InvalidSizeMatrixException">If rows or cols are negative throws exception</exception>
        public static int[,] Generate(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0)
            {
                throw new InvalidSizeMatrixException("Rows and columns must be positive");
            }

            var random = new System.Random();
            var resultMatrix = new int[rows, columns];
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    resultMatrix[row, col] = random.Next();
                }
            }

            return resultMatrix;
        }
    }
}