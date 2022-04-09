namespace ThreadPool.Tests;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using System;
using NUnit.Framework;

[TestFixture]
public class MyThreadPoolTests
{
    private const int CountOfTasks = 10;
    private readonly IMyTask<int>[] _tasks = new IMyTask<int>[CountOfTasks];
    private MyThreadPool _threadPool;

    [SetUp]
    public void Setup()
    {
        Console.Write("Setup tut");
        _threadPool = new MyThreadPool(CountOfTasks);
        for (var i = 0; i < CountOfTasks; i++)
        {
            var index = i;
            _tasks[index] =
                _threadPool.Submit(() => index);
        }
    }

    [Test]
    public void IsCompletedTest()
    {
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
        var task2 = task1.ContinueWith(j => 1 / j);
        var task3 = task2.ContinueWith(j => j.ToString());

        Assert.Throws<AggregateException>(() => _ = task2.Result);
        Assert.Throws<AggregateException>(() => _ = task3.Result);
    }

    [Test]
    public void ThreadPoolShouldCalculateTasks()
    {
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
            Assert.Throws<ThreadPoolShutdownException>(() => _threadPool.Submit(() => 0));
        }
    }

    [Test]
    public void ContinueWithTasksShouldRaiseExceptionAfterShutdown()
    {
        _threadPool.Shutdown();
        for (var i = 0; i < CountOfTasks; i++)
        {
            Assert.Throws<ThreadPoolShutdownException>(() => _tasks[i].ContinueWith(_ => 0));
        }
    }

    [Test]
    public void ContinueWithShouldCalculateTasks()
    {
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
    public void ContinueWithShouldCompleteTheAssignedTasks()
    {
        var continueTasks = new IMyTask<int>[CountOfTasks];
        for (var i = 0; i < CountOfTasks; i++)
        {
            continueTasks[i] = _tasks[i].ContinueWith(x => x * x);
        }

        _threadPool.Shutdown();

        for (var i = 0; i < CountOfTasks; i++)
        {
            Assert.AreEqual(i * i, continueTasks[i].Result);
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

    [Test]
    [NonParallelizable]
    public void MultiThreadingTest()
    {
        var anotherPool = new MyThreadPool(CountOfTasks);
        var array = new int[CountOfTasks];

        Parallel.ForEach(Enumerable.Range(0, CountOfTasks), i =>
        {
            var localIndex = i;
            switch (i)
            {
                case 4:
                    Thread.Sleep(5000);
                    anotherPool.Shutdown();
                    break;
                case < 4:
                    anotherPool.Submit(() => localIndex)
                        .ContinueWith(_ =>
                        {
                            array[localIndex] = Interlocked.Increment(ref localIndex);
                            return 1;
                        });
                    break;
                default:
                    Thread.Sleep(10000);
                    Assert.Throws<ThreadPoolShutdownException>(() =>
                    {
                        anotherPool.Submit(() => localIndex);
                    });
                    break;
            }
        });
        
        Thread.Sleep(5000);

        for (var i = 0; i < 4; ++i)
        {
            Assert.AreEqual(i + 1, array[i]);
        }

        foreach (var item in array)
        {
            Console.WriteLine(item);
        }

        Assert.AreEqual(0, array[4]);
    }
}