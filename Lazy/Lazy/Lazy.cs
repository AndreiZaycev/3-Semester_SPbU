using System;

namespace Lazy;

/// <summary>
/// Abstract description of lazy evaluation
/// </summary>
public abstract class Lazy<T> : ILazy<T>
{
    protected Func<T> function;
    protected T result;
    protected bool isComputed;

    /// <summary>
    /// Creates a new Lazy with a specific function
    /// </summary>
    /// <param name="func">Function that compute expression</param>
    /// <exception cref="ArgumentNullException">Throws when function is null</exception>
    protected Lazy(Func<T> func) => function = func ?? throw new ArgumentNullException(nameof(func));

    /// <inheritdoc/>
    public abstract T Get();
}