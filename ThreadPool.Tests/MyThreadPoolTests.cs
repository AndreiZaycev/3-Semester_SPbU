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
    public void AggregateExceptionTest()
    {
        var pool = new MyThreadPool(10);
        var task1 = pool.Submit(() => 0);
       // Thread.Sleep(2000);
        var task2 = task1.ContinueWith(j => 1 / j);

        Assert.Throws<AggregateException>(() => _ = task2.Result);
    }
}