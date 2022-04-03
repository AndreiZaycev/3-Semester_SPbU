namespace ThreadPool;

#nullable enable
using System;
using System.Collections.Concurrent;
using System.Threading;

/// <summary>
/// Implementation of thread pool with executing tasks <see cref="IMyTask{TResult}"/>
/// </summary>
public class MyThreadPool
{
    private readonly BlockingCollection<Action> _actions = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly object _actionQueueLocker = new();
    private readonly ManualResetEvent _manualReset = new(false);
    private readonly Thread[] _threads;

    public MyThreadPool(int threadsCount)
    {
        if (threadsCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(threadsCount));
        }

        _threads = new Thread[threadsCount];

        for (var i = 0; i < threadsCount; i++)
        {
            _threads[i] = CreateOneThread();
            _threads[i].Start();
        }
    }

    /// <summary>
    /// Adds action in collection
    /// </summary>
    /// <param name="action">Action</param>
    /// <exception cref="ThreadPoolShutdownException">Cancellation was requested</exception>
    private void AddAction(Action action)
    {
        if (_cancellationTokenSource.IsCancellationRequested)
        {
            throw new ThreadPoolShutdownException();
        }

        _actions.Add(action);
        _manualReset.Set();
    }

    /// <summary>
    /// Creates thread 
    /// </summary>
    /// <returns>New thread</returns>
    private Thread CreateOneThread()
    {
        var thread = new Thread(() =>
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Action? task;

                while (!_actions.TryTake(out task) && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _manualReset.Reset();
                    _manualReset.WaitOne();
                }

                task?.Invoke();
            }
        })
        {
            IsBackground = true
        };
        return thread;
    }

    /// <summary>
    /// Adds task to the counting collection
    /// </summary>
    /// <param name="supplier">Function to calculate</param>
    /// <typeparam name="TResult">Type of result</typeparam>
    /// <returns>New task</returns>
    /// <exception cref="ThreadPoolShutdownException">Cancellation was requested</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> supplier)
    {
        lock (_cancellationTokenSource)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolShutdownException();
            }


            var task = new MyTask<TResult>(supplier, this);
            AddAction(task.Run);
            return task;
        }
    }

    /// <summary>
    /// Shutdowns thread pool
    /// </summary>
    /// <exception cref="ThreadPoolShutdownException">Cancellation already was requested</exception>
    public void Shutdown()
    {
        lock (_actionQueueLocker)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolShutdownException();
            }

            _cancellationTokenSource.Cancel();
            _manualReset.Set();

            foreach (var thread in _threads)
            {
                thread.Join();
            }
        }
    }

    /// <summary>
    /// Implementation of interface <see cref="IMyTask{TResult}"/>
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    private class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly BlockingCollection<Action> _continuations = new();
        private readonly MyThreadPool _threadPool;
        private Func<TResult>? _supplier;
        private TResult? _result;
        private Exception? _isAggregateExceptionThrown;
        private readonly ManualResetEvent _isResultReadyEvent = new(false);

        /// <inheritdoc />
        public bool IsCompleted { get; private set; }

        public MyTask(Func<TResult> supplier, MyThreadPool threadPool)
        {
            _threadPool = threadPool;
            _supplier = supplier;
        }

        /// <inheritdoc />
        public TResult? Result
        {
            get
            {
                _isResultReadyEvent.WaitOne();

                if (_isAggregateExceptionThrown != null)
                {
                    throw new AggregateException(_isAggregateExceptionThrown);
                }

                return _result;
            }
        }

        /// <summary>
        /// Calculates the result
        /// </summary>
        public void Run()
        {
            try
            {
                _result = _supplier!();
                IsCompleted = true;
                _supplier = null;
            }
            catch (Exception exception)
            {
                _isAggregateExceptionThrown = new AggregateException(exception);
            }

            _isResultReadyEvent.Set();

            while (_continuations.Count != 0)
            {
                _threadPool._actions.Add(_continuations.Take());
            }
        }

        /// <inheritdoc/>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
        {
            lock (_threadPool._cancellationTokenSource)
            {
                if (_threadPool._cancellationTokenSource.IsCancellationRequested)
                {
                    throw new ThreadPoolShutdownException();
                }

                var task = new MyTask<TNewResult>(() => supplier(Result!), _threadPool);

                if (IsCompleted)
                {
                    _threadPool.AddAction(task.Run);
                }
                else
                {
                    _continuations.Add(task.Run);
                }

                return task;
            }
        }
    }
}