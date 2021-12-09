using System;
using System.Threading;

namespace Lazy;

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
        if (isComputed)
        {
            return result;
        }

        lock (_lock)
        {
            if (isComputed)
            {
                return result;
            }
            result = function();
            function = null;
            isComputed = true;
        }

        return result;
    }
}