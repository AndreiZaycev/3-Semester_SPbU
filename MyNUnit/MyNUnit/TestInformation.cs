namespace MyNUnit;

/// <summary>
/// Implementation of the information about test
/// </summary>
public class TestInformation
{
    /// <summary>
    /// Name of the test
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Reason for ignoring test
    /// </summary>
    public string? IgnoreReason { get; }

    /// <summary>
    /// Running time of test
    /// </summary>
    public long Time { get; }

    /// <summary>
    /// Information about the test execution: can be errored, ignored, passed or failed
    /// </summary>
    public string Result { get; }

    /// <summary>
    /// Error message of the test
    /// </summary>
    public string? ErrorMessage { get; }

    public TestInformation(string name, string result, string? ignore = null, string? errorMessage = null, long time = 0)
    {
        Name = name;
        Result = result;
        IgnoreReason = ignore;
        ErrorMessage = errorMessage;
        Time = time;
    }
}