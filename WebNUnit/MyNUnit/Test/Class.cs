namespace Test;
using MyNUnit.Attributes;
public static class Class
{
    public static int counterBefore { get; set; }
    public static int counterAfter { get; set; }

    [BeforeClass]
    public static void BeforeClass()
    {
        counterBefore += 1;
    }

    [Before]
    public static void Before()
    {
        counterBefore += 1;
    }

    [AfterClass]
    public static void AfterClass()
    {
        counterAfter += 1;
    }

    [After]
    public static void After()
    {
        counterAfter += 1;
    }
    
    [Test]
    public static void PassingTest()
    {
        
    }

    [Test]
    public static string IncorrectTest()
    {
        return "abacaba";
    }

    [Test]
    public static void FailingTest()
    {
        throw new Exception();
    }
    
    [Test(typeof(NullReferenceException))]
    public static void NullReferenceTest()
    {
        throw new NullReferenceException();
    }
    
    [Test(typeof(OutOfMemoryException))]
    public static void OutOfMemoryTest()
    {
        throw new NullReferenceException();
    }

    [Test(Ignore = "This test is ignored because he does not respect Russians")]
    public static void IgnoringTest()
    {
        var str = "I am american agent";
    } 
}