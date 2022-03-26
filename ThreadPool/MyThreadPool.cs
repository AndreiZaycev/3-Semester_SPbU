#nullable enable
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ThreadPool
{
    /// <summary>
    /// Implementation of thread pool with executing tasks <see cref="IMyTask{TResult}"/>
    /// </summary>
    public class MyThreadPool
    {
        private readonly BlockingCollection<Action> _actions = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly object _actionQueueLocker = new();

        public MyThreadPool(int threadsCount)
        {
            if (threadsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(threadsCount));
            }

            var threads = new Thread[threadsCount];

            for (var i = 0; i < threadsCount; i++)
            {
                threads[i] = CreateOneThread();
                threads[i].Start();
            }
        }

        /// <summary>
        /// Adds action in collection
        /// </summary>
        /// <param name="action">Action</param>
        /// <exception cref="ThreadPoolShutdownException">Cancellation was requested</exception>
        private void AddAction(Action action)
        {
            lock (_actionQueueLocker)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    throw new ThreadPoolShutdownException();
                }

                _actions.Add(action);
                
                Monitor.Pulse(_actionQueueLocker);
            }
        }

        /// <summary>
        /// Creates thread 
        /// </summary>
        /// <returns>New thread</returns>
        private Thread CreateOneThread()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    Action task;
                    lock (_actionQueueLocker)
                    {
                        while (_actions.Count == 0)
                        {
                            if (_actions.IsAddingCompleted)
                            {
                                return;
                            }

                            Monitor.Wait(_actionQueueLocker);
                        }

                        
                        task = _actions.Take();
                    }

                    task.Invoke();
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
        public IMyTask<TResult> Submit<TResult>(Func<TResult>? supplier)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolShutdownException();
            }

            var task = new MyTask<TResult>(supplier, this);
            AddAction(task.Run);
            return task;
        }

        /// <summary>
        /// Shutdowns thread pool
        /// </summary>
        /// <exception cref="ThreadPoolShutdownException">Cancellation already was requested</exception>
        public void Shutdown()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolShutdownException();
            }

            lock (_actionQueueLocker)
            {
                _cancellationTokenSource.Cancel();
            }

            _actions.CompleteAdding();
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
            private TResult _result;
            private Exception? _isAggregateExceptionThrown;
            private readonly ManualResetEvent _isResultReadyEvent = new(false);
            private readonly object _continuationQueueLocker = new();

            /// <inheritdoc />
            public bool IsCompleted { get; private set; }

            public MyTask(Func<TResult>? supplier, MyThreadPool threadPool)
            {
                _threadPool = threadPool;
                _supplier = supplier;
            }

            /// <inheritdoc />
            public TResult Result
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
                    lock (_threadPool._cancellationTokenSource)
                    {
                        if (_threadPool._cancellationTokenSource.IsCancellationRequested)
                        {
                            throw new ThreadPoolShutdownException();
                        }
                    }
                    
                    _result = _supplier!();
                    IsCompleted = true;
                    _supplier = null;
                }
                catch (Exception exception)
                {
                    _isAggregateExceptionThrown = new AggregateException(exception);
                }
                finally
                {
                    _isResultReadyEvent.Set();
                    lock (_continuationQueueLocker)
                    {
                        while (_continuations.Count != 0)
                        {
                            lock (_threadPool._actionQueueLocker)
                            {
                                _threadPool._actions.Add(_continuations.Take());
                            }
                        }
                    }
                }
            }

            /// <inheritdoc/>
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
            {
                lock (_continuationQueueLocker)
                {
                    var task = new MyTask<TNewResult>(() => supplier(Result), _threadPool);

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
}