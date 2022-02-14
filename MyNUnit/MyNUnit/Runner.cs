namespace MyNUnit;

using Attributes;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// Implementation of the test framework
/// </summary>
public static class Runner
{
    /// <summary>
    /// Collection with the information about tests
    /// </summary>
    public static BlockingCollection<TestInformation>? TestInformation { get; private set; }

    private enum TestStatus
    {
        Ignored,
        Failed,
        Passed,
        Errored
    };

    /// <summary>
    /// Runs test
    /// </summary>
    /// <param name="path">path to the directory with the test class</param>
    public static void Run(string path)
    {
        var types = Directory
            .GetFiles(path, "*.dll", SearchOption.AllDirectories)
            .Select(Assembly.LoadFrom)
            .ToHashSet()
            .SelectMany(a => a.ExportedTypes);
        TestInformation = new BlockingCollection<TestInformation>();
        Parallel.ForEach(types, type =>
        {
            RunMethodsWithAttribute<BeforeClass>(type);
            RunMethodsWithAttribute<Test>(type);
            RunMethodsWithAttribute<AfterClass>(type);
        });
    }

    private static bool Check(MethodInfo methodInfo)
    {
        var errorMessage = "";

        if (methodInfo.ReturnType != typeof(void))
        {
            errorMessage = $"{methodInfo.Name} should return void";
        }

        if (methodInfo.GetParameters().Length > 0)
        {
            errorMessage = $"{methodInfo.Name} should have no parameters";
        }

        if (errorMessage == "") return false;
        TestInformation?.Add(new TestInformation(methodInfo.Name, TestStatus.Errored.ToString(), null,
            errorMessage));
        return true;
    }

    private static void ExecuteTestMethod(MethodInfo methodInfo)
    {
        if (Check(methodInfo))
        {
            return;
        }

        var attributes = Attribute.GetCustomAttribute(methodInfo, typeof(Test)) as Test;

        if (attributes?.Ignore != null)
        {
            TestInformation?.Add(new TestInformation(methodInfo.Name, TestStatus.Ignored.ToString(), attributes.Ignore));
            return;
        }

        var instance = Activator.CreateInstance(methodInfo.DeclaringType?.BaseType!);

        RunMethodsWithAttribute<Before>(methodInfo.DeclaringType!, instance);

        var timer = Stopwatch.StartNew();
        TestInformation testInformation;

        void GetInformation(bool expression)
        {
            if (expression)
            {
                testInformation = new TestInformation(methodInfo.Name, TestStatus.Failed.ToString(),
                    null, null, timer.ElapsedMilliseconds);
            }
            else
            {
                testInformation = new TestInformation(methodInfo.Name, TestStatus.Passed.ToString(),
                    null, null, timer.ElapsedMilliseconds);
            }
        }

        try
        {
            methodInfo.Invoke(instance, null);
            timer.Stop();
            GetInformation(attributes?.Expected != null);
        }
        catch (Exception exception)
        {
            timer.Stop();
            GetInformation(attributes?.Expected != exception.InnerException?.GetType());
        }

        RunMethodsWithAttribute<After>(methodInfo.DeclaringType!, instance);

        TestInformation?.Add(testInformation);
    }

    private static void RunMethodsWithAttribute<T>(Type type, object? instance = null)
    {
        var methodsWithAttribute = type
            .GetTypeInfo()
            .DeclaredMethods
            .Where(methodInfo => Attribute.IsDefined(methodInfo, typeof(T)));

        var attributes = new[]
        {
            typeof(BeforeClass),
            typeof(AfterClass),
            typeof(Before),
            typeof(After)
        };

        Action<MethodInfo> runMethod = typeof(T) switch
        {
            { } attribute when attribute == typeof(Test) => ExecuteTestMethod,
            { } attribute when attributes.Any(a => a == attribute) => methodInfo =>
            {
                if (Check(methodInfo))
                {
                    return;
                }

                methodInfo.Invoke(instance, null);
            },
            _ => throw new ArgumentException("Wrong attribute")
        };

        Parallel.ForEach(methodsWithAttribute, runMethod);
    }
}