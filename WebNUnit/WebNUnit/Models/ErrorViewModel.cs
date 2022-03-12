namespace WebNUnit.Models;

/// <summary>
/// Implementation of error view model
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Request id of error
    /// </summary>
    public string? RequestId { get; init; }

    /// <summary>
    /// Check request id is not empty
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
