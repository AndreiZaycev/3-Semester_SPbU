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
    public void SubmittedTasksShouldRaiseExceptionAfterShutdown()
    {
        _threadPool.Shutdown();
        for (var i = 0; i < CountOfTasks; i++)
        {
            Assert.Throws<ThreadPoolShutdownException>(() => _threadPool.Submit(() => 0));
        }
    }
}