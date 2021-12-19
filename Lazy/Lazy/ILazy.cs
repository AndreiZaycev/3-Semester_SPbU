namespace Lazy;

/// <summary>
/// Lazy evaluation structure
/// </summary>
public interface ILazy<out T>
{
    /// <summary>
    /// Gets calculated value 
    /// </summary>
    /// <returns>Type of returning object</returns>
    public T Get();
}