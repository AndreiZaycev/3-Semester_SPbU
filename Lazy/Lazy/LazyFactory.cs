using System;

namespace Lazy
{
    /// <summary>
    /// Implementation of factory concurrent and standard lazy evaluation
    /// </summary>
    public class LazyFactory
    {
        /// <summary>
        /// Creates standard lazy evaluation 
        /// </summary>
        /// <param name="func">Function that compute expression</param>
        /// <returns>Returns new instance of LazyEvaluation</returns>
        public static Lazy<T> CreateLazyEvaluation<T>(Func<T> func)
            => new LazyEvaluation<T>(func);

        /// <summary>
        /// Creates lazy concurrent evaluation 
        /// </summary>
        /// <param name="func">Function that compute expression</param>
        /// <returns>Returns new instance of LazyEvaluation</returns>
        public static Lazy<T> CreateLazyConcurrent<T>(Func<T> func)
            => new LazyConcurrent<T>(func);
    }
}