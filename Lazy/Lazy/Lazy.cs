using System;

namespace Lazy
{
    /// <summary>
    /// Abstract description of lazy evaluation
    /// </summary>
    public abstract class Lazy<T> : ILazy<T>
    {
        protected Func<T> Function;
        protected T Result;
        protected bool IsComputed;

        /// <summary>
        /// Creates a new Lazy with a specific function
        /// </summary>
        /// <param name="func">Function that compute expression</param>
        /// <exception cref="ArgumentNullException">Throws when function is null</exception>
        protected Lazy(Func<T> func) => Function = func ?? throw new ArgumentNullException(nameof(func));

        /// <inheritdoc/>
        public abstract T Get();
    }
}