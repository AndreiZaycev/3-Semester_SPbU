using System;

namespace Matrix;

/// <summary>
/// Exception when trying to multiply matrices of the wrong size
/// </summary>
public class InvalidSizesMultiplicationException : Exception
{
    public InvalidSizesMultiplicationException(string message)
        : base(message)
    {
    }
}