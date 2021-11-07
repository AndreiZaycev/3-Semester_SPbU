namespace Matrix
{
    /// <summary>
    /// Implementation of print for matrices
    /// </summary>
    public class PrintMatrix
    {
        /// <summary>
        /// Prints matrix in file
        /// </summary>
        /// <param name="matrix">Matrix to print</param>
        /// <param name="path">Path of file to print matrix</param>
        public static void Print(int[,] matrix, string path)
        {
            var rowLine = "";
            for (var row = 0; row < matrix.GetLength(0); row++)
            {
                for (var column = 0; column < matrix.GetLength(1); column++)
                {
                    rowLine += $"{matrix[row, column]} ";
                }

                rowLine = $"{rowLine.TrimEnd()}\n";
            }

            System.IO.File.WriteAllText(path, rowLine);
        }
    }
}