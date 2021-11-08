namespace Lazy
{
    /// <summary>
    /// Lazy evaluation structure
    /// </summary>
    public interface ILazy<out T>
    {
        /// <summary>
        /// Gets calculated value 
        /// </summary>
        /// <returns>Returns generic type T</returns>
        public T Get();
    }
}