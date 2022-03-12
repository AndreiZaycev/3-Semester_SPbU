namespace WebNUnit.Models;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Implementation of test view model
/// </summary>
public class TestViewModel
{
    /// <summary>
    /// Test id
    /// </summary>
    [Key]
    public int Id { get; set; } 
    
    /// <summary>
    /// Test name
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Test result
    /// </summary>
    public string? Result { get; init; }

    /// <summary>
    /// Ignore reason
    /// </summary>
    public string? IgnoreReason { get; init; }

    /// <summary>
    /// Test runtime
    /// </summary>
    public long Time { get; init; }

    /// <summary>
    /// Test start time
    /// </summary>
    public DateTime StartTime { get; init; }
}