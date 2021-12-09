using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace Lazy.Tests;

public class Tests
{
    private static List<TestCaseData> LazyData()
    {
        return new List<TestCaseData>
        {
            new("bebr"),
            new(1337),
            new("ip tcp"),
            new("gaziki"),
            new("lol"),
            new(1.256)
        };
    }
    
    private static IEnumerable<TestCaseData> LazyConcurrentAndEvaluation()
    {
            int iterator = 3;
            int counter = 3;
            yield return new TestCaseData(LazyFactory.CreateLazyConcurrent(() => ++iterator));
            yield return new TestCaseData(LazyFactory.CreateLazyEvaluation(() => Interlocked.Increment(ref counter)));
    }

    [TestCaseSource(nameof(LazyData))]
    public void LaziesShouldReturnsSameObjects(object item)
    {
        var lazyEvaluation = LazyFactory.CreateLazyEvaluation(() => item);
        var lazyConcurrent = LazyFactory.CreateLazyConcurrent(() => item);
        Assert.AreEqual(lazyConcurrent.Get(), lazyEvaluation.Get());
    }

    [Test]
    public void NullExceptionShouldBeRaised()
    {
        Assert.Catch<ArgumentNullException>(() => LazyFactory.CreateLazyConcurrent<Object>(null));
        Assert.Catch<ArgumentNullException>(() => LazyFactory.CreateLazyEvaluation<Object>(null));
    }

    [TestCaseSource(nameof(LazyConcurrentAndEvaluation))]
    public static void LazyEvaluationShouldComputesExpressionOnlyOnce<T>(ILazy<T> lazy)
    {
        for (var i = 0; i < 10; i++)
        {
            Assert.AreEqual(4, lazy.Get());
        }
    }

    [TestCaseSource(nameof(LazyConcurrentAndEvaluation))]
    public static void LazyConcurrentShouldComputesExpressionOnlyOnce<T>(ILazy<T> lazy)
    {
        for (var i = 0; i < 10; i++)
        {
            Assert.AreEqual(4, lazy.Get());
        }
    }

    [Test]
    public static void LazyConcurrentShouldNotHaveRaces()
    {
        var iterator = 0;
        var result = LazyFactory.CreateLazyConcurrent(() => Interlocked.Increment(ref iterator));
        var threads = new Thread[1000];
        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                for (var j = 0; j < 100; j++)
                {
                    Assert.AreEqual(1, result.Get());
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}