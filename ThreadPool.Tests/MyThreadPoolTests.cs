using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ThreadPool;

namespace ThreadPool.Tests
{
    [TestFixture]
    public class MyThreadPoolTests
    {
        private const int CountOfTasks = 1000;
        private readonly IMyTask<int>[] _tasks = new IMyTask<int>[CountOfTasks];
        private MyThreadPool _threadPool;
        private readonly ManualResetEvent _resetEvent = new(false);

        [SetUp]
        public void Setup()
        {
            _threadPool = new MyThreadPool(CountOfTasks);
            for (var i = 0; i < CountOfTasks; i++)
            {
                var index = i;
                _tasks[index] =
                    _threadPool.Submit(() =>
                    {
                        _resetEvent.WaitOne();
                        return index;
                    });
            }
        }

        [Test]
        public void IsCompletedTest()
        {
            _resetEvent.Set();
            foreach (var task in _tasks)
            {
                Assert.IsTrue(task.IsCompleted);
            }
        }

        [Test]
        public void AggregateExceptionTest()
        {
            var pool = new MyThreadPool(10);
            var task1 = pool.Submit(() => 0);
            var task2 = task1.ContinueWith((j) => 1 / j);
            var task3 = task2.ContinueWith((j) => j.ToString());


            Assert.Throws<AggregateException>(() => _ = task2.Result);
            Assert.Throws<AggregateException>(() => _ = task3.Result);
        }

        [Test]
        public void ThreadPoolShouldCalculateTasks()
        {
            _resetEvent.Set();
            for (var i = 0; i < CountOfTasks; i++)
            {
                Assert.AreEqual(i, _tasks[i].Result);
            }
        }

        [Test]
        public void SubmittedTasksShouldRaiseExceptionAfterShutdown()
        {
            _threadPool.Shutdown();
            for (var i = 0; i < CountOfTasks; i++)
            {
                Assert.Throws<ThreadPoolShutdownException>(() => _ = _tasks[i].Result);
            }
        }

        [Test]
        public void ContinueWithShouldCalculateTasks()
        {
            _resetEvent.Set();
            var continueTasks = new IMyTask<int>[CountOfTasks];
            for (var i = 0; i < CountOfTasks; i++)
            {
                continueTasks[i] = _tasks[i].ContinueWith(x => x + 1);
            }

            for (var i = 0; i < CountOfTasks; i++)
            {
                Assert.AreEqual(i + 1, continueTasks[i].Result);
            }
        }

        [Test]
        public void ContinueWithShouldThrowExceptionAfterShutdown()
        {
            _resetEvent.Set();
            var continueTasks = new IMyTask<int>[CountOfTasks];
            for (var i = 0; i < CountOfTasks; i++)
            {
                continueTasks[i] = _tasks[i].ContinueWith(x =>
                {
                    _resetEvent.WaitOne();
                    return x * x;
                });
            }

            _threadPool.Shutdown();
            _resetEvent.Set();

            for (var i = 0; i < CountOfTasks; i++)
            {
                Assert.Throws<ThreadPoolShutdownException>(() => _ = continueTasks[i].Result);
            }
        }

        [Test]
        public void ThreadPoolShouldRaiseExceptionWhenNumberOfThreadsLessOrEqualToZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new MyThreadPool(-228);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new MyThreadPool(0);
            });
        }
    }
}