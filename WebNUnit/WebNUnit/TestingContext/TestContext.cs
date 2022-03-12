namespace WebNUnit.TestingContext;
using Microsoft.EntityFrameworkCore;
using Models;

/// <summary>
/// Implementation of test context 
/// </summary>
[Keyless]
public class TestContext: DbContext
{
    /// <summary>
    /// History DbSet
    /// </summary>
    public DbSet<LoadedAssembliesViewModel>? AssembliesHistory { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Database;Trusted_Connection=True;");
    }
}