namespace MyNUnit.Attributes;

/// <summary>
/// A method marked with this attribute is considered a test
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Test : Attribute
{
    public Test(Type? expected = null, string? ignore = null)
    {
        Expected = expected;
        Ignore = ignore;
    }
    public Type? Expected { get; }
    public string? Ignore { get; set; }
}