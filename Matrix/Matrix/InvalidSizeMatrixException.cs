using System;

namespace Matrix
{
    /// <summary>
    /// Exception when sizes of matrices are negative
    /// </summary>
    public class InvalidSizeMatrixException : Exception
    {
        public InvalidSizeMatrixException(string message)
            : base(message)
        {
        }
    }
}