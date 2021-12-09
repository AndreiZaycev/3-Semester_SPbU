namespace Matrix;

/// <summary>
/// Multiplication interface 
/// </summary>
public interface IMultiplication
{
    /// <summary>
    /// Multiplies two Matrices and return result Matrix
    /// </summary>
    /// <param name="firstMatrix">First matrix in multiplication</param>
    /// <param name="secondMatrix">Second matrix in multiplication</param>
    /// <returns>Result Matrix</returns>
    public Matrix Multiply(Matrix firstMatrix, Matrix secondMatrix);
}