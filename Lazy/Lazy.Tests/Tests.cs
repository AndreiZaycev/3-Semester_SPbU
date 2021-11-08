using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Lazy.Tests
{
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

        [Test]
        public static void LazyEvaluationShouldComputesExpressionOnlyOnce()
        {
            var iterator = 2;
            var result = LazyFactory.CreateLazyEvaluation(() => iterator * iterator);
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(4, result.Get());
            }
        }

        [Test]
        public static void LazyConcurrentShouldComputesExpressionOnlyOnce()
        {
            var iterator = 2;
            var result = LazyFactory.CreateLazyConcurrent(() => iterator * iterator);
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(4, result.Get());
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
}