using System;
using NUnit.Framework;

namespace ThreadPool.Tests;

[TestFixture]
public class MyThreadPoolTests
{
    private const int CountOfTasks = 1000;
    private readonly IMyTask<int>[] _tasks = new IMyTask<int>[CountOfTasks];
    private MyThreadPool _threadPool;

    [SetUp]
    public void Setup()
    {
        _threadPool = new MyThreadPool(CountOfTasks);
        for (var i = 0; i < CountOfTasks; i++)
        {
            var index = i;
            _tasks[index] =
                _threadPool.Submit(() => { return index; });
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
    public void ContinueWithShouldThrowExceptionAfterShutdown()
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
}