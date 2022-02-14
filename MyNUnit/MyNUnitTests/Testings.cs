using System.Linq;
using MyNUnit.Attributes;
using NUnit.Framework;
using Test;

namespace MyNUnitTests;

public class Testings
{
    [OneTimeSetUp]
    public void Run()
    {
        MyNUnit.Runner.Run("..\\..\\..\\..\\Test");
    }

    [Test]
    public void TestAllAttributesWorkCorrectly()
    {
        Assert.AreEqual(5, Class.counterBefore);
        Assert.AreEqual(5, Class.counterAfter);
    }

    [Test]
    public void PassingTestShouldWorkCorrectly()
    {
        Assert.IsTrue(MyNUnit.Runner.TestInformation.Any(testInfo => testInfo.Name == "PassingTest"
                                                                     && testInfo.Result == "Passed"));
    }

    [Test]
    public void IgnoreTestShouldWorkCorrectly()
    {
        const string message = "This test is ignored because he does not respect Russians";
        Assert.IsTrue(MyNUnit.Runner.TestInformation.Any(testInfo => testInfo.Name == "IgnoringTest"
                                                                     && testInfo.Result == "Ignored"
                                                                     && testInfo.IgnoreReason == message));
    }

    [Test]
    public void IncorrectTestShouldWorkCorrectly()
    {
        Assert.IsTrue(MyNUnit.Runner.TestInformation.Any(testInfo => testInfo.Name == "IncorrectTest"
                                                                     && testInfo.Result == "Errored"
                                                                     && testInfo.ErrorMessage ==
                                                                     "IncorrectTest should return void"));
    }

    [Test]
    public void FailingTestShouldWorkCorrectly()
    {
        Assert.IsTrue(MyNUnit.Runner.TestInformation.Any(testInfo => testInfo.Name == "FailingTest"
                                                                     && testInfo.Result == "Failed"));
    }

    [Test]
    public void NullReferenceTestShouldWorkCorrectly()
    {
        Assert.IsTrue(MyNUnit.Runner.TestInformation.Any(testInfo => testInfo.Name == "NullReferenceTest"
                                                                     && testInfo.Result == "Passed"));
    }

    [Test]
    public void ExceptionTestShouldWorkCorrectly()
    {
        Assert.IsTrue(MyNUnit.Runner.TestInformation.Any(testInfo => testInfo.Name == "OutOfMemoryTest"
                                                                     && testInfo.Result == "Failed"));
    }

    [Test]
    public void TimeShouldBeNonNegative()
    {
        Assert.IsTrue(MyNUnit.Runner.TestInformation.All(testInfo => testInfo.Time >= 0));
    }
}