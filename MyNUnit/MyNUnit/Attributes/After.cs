namespace MyNUnit.Attributes;

/// <summary>
/// The method marked with this attribute will work after each test
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class After : Attribute
{
}
