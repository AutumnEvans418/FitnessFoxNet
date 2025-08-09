using FitnessFox.Data.Foods;
using FitnessFox.Data.Goals;
using FitnessFox.Data.Vitals;
using Microsoft.EntityFrameworkCore;

namespace FitnessFox.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<UserVital> UserVitals { get; set; }
        public DbSet<UserGoal> UserGoals { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeFood> RecipeFoods { get; set; }
        public DbSet<UserMeal> UserMeals { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Recipe>()
                .HasMany(r => r.Foods)
                .WithOne(rf => rf.Recipe)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("AspNetUsers");
            });

            base.OnModelCreating(builder);
        }

        void UpdateState()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IEntityAudit && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((IEntityAudit)entityEntry.Entity).DateModified = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((IEntityAudit)entityEntry.Entity).DateCreated = DateTime.Now;
                }
            }
        }


        public override int SaveChanges()
        {
            UpdateState();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateState();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
