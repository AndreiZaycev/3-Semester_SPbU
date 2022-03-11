using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebNUnit.Models;

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
    public string Name { get; set; }

    /// <summary>
    /// Tests 
    /// </summary>
    public List<TestViewModel> Tests { get; set; } = new();

}