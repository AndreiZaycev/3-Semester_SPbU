namespace ThreadPool;

#nullable enable
using System;

/// <summary>
/// interface describing the task in the thread pool
/// </summary>
/// <typeparam name="TResult">Task result type.</typeparam>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// Notifies about the end of counting something
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of the task
    /// </summary>
    TResult? Result { get; }

    /// <summary>
    /// Adds a new task depends on the other task 
    /// </summary>
    /// <param name="supplier">New function in the task</param>
    /// <typeparam name="TNewResult">New type of returned result</typeparam>
    /// <returns>Returns new task</returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier);
}