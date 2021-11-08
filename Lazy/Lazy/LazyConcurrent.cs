using System;
using System.Threading;

namespace Lazy
{
    /// <summary>
    /// Concurrent implementation of ILazy interface
    /// </summary>
    /// <typeparam name="T">Generic type of returning instance</typeparam>
    public class LazyConcurrent<T> : Lazy<T>
    {
        private readonly object _lock = new();

        /// <inheritdoc/>
        public LazyConcurrent(Func<T> func)
            : base(func)
        {
        }

        /// <inheritdoc/>
        public override T Get()
        {
            if (IsComputed)
            {
                return Result;
            }

            lock (_lock)
            {
                if (Volatile.Read(ref IsComputed))
                {
                    return Result;
                }

                Result = Function();
                Function = null;
                IsComputed = true;
                return Result;
            }
        }
    }
}