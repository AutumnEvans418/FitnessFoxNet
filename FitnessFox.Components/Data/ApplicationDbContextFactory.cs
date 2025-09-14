using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using FitnessFox.Data;

namespace FitnessFox.Components.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Use SQLite for migrations, matching MauiProgram.cs
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "database.dat");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
