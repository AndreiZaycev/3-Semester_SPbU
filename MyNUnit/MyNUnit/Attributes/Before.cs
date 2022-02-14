namespace MyNUnit.Attributes;

/// <summary>
/// The method marked with this attribute will work before each test
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Before : Attribute
{
}