namespace MyNUnit.Attributes;

/// <summary>
/// The method marked with this attribute will work before each test class
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeClass : Attribute
{
}