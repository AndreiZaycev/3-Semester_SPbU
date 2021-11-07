namespace Matrix
{
    /// <summary>
    /// Implementation of read for matrices
    /// </summary>
    public class ReadMatrix
    {
        /// <summary>
        /// Reads matrix from file
        /// </summary>
        /// <param name="path">Path of file to read</param>
        /// <returns>Returns read matrix</returns>
        public static int[,] Read(string path)
        {
            var allLines = System.IO.File.ReadAllLines(path);
            var firstDimension = allLines[0].Split(" ").Length;
            var secondDimension = allLines.Length;
            var resultMatrix = new int[firstDimension, secondDimension];
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
}