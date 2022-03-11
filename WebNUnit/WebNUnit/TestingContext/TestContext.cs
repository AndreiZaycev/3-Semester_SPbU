using Microsoft.EntityFrameworkCore;
using WebNUnit.Models;

namespace WebNUnit.TestingContext;

[Keyless]
public class TestContext: DbContext
{
    /// <summary>
    /// History DbSet
    /// </summary>
    public DbSet<LoadedAssembliesViewModel> AssembliesHistory { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Database;Trusted_Connection=True;");
    }
}