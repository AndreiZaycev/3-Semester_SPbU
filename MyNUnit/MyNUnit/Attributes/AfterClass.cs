namespace MyNUnit.Attributes;

/// <summary>
/// The method marked with this attribute will work after each test class
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterClass : Attribute
{
}