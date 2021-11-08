using System;

namespace Lazy
{
    /// <summary>
    /// Standard implementation of ILazy interface
    /// </summary>
    /// <typeparam name="T">Generic type of returning instance</typeparam>
    public class LazyEvaluation<T> : Lazy<T>
    {
        /// <inheritdoc/>
        public LazyEvaluation(Func<T> func)
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

            Result = Function();
            Function = null;
            IsComputed = true;
            return Result;
        }
    }
}