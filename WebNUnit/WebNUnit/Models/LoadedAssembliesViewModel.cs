namespace WebNUnit.Models;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Implementation of loaded assemblies view model 
/// </summary>
public class LoadedAssembliesViewModel
{
    /// <summary>
    /// Id of assembly
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Name of assembly
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Tests 
    /// </summary>
    public List<TestViewModel> Tests { get; } = new();
}