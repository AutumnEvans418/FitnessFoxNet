using AutoFixture;
using FitnessFox.Components.Services;
using FitnessFox.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FitnessFox.Tests
{
    public abstract class DbTestBase<T> : IDisposable
    {
        public SqliteConnection Connection { get; set; }
        public Fixture Fixture { get; set; }

        public T Subject { get; set; }
        public ApplicationDbContext Db { get; set; }
        public IAuthenticationService AuthenticationService { get; set; }

        protected DbTestBase()
        {
            Connection = new SqliteConnection("Filename=:memory:");
            Connection.Open();
            var fixture = new Fixture();
            fixture.Customize(new AutoFixture.AutoNSubstitute.AutoNSubstituteCustomization());
            Fixture = fixture;
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(Connection).Options;
            var db = new ApplicationDbContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            fixture.Inject(db);

            var user = fixture.Create<ApplicationUser>();
            db.Users.Add(user);
            db.SaveChanges();
            this.Db = db;

            AuthenticationService = fixture.Freeze<IAuthenticationService>();
            AuthenticationService.GetUserAsync().Returns(user);

            Setup();

            Subject = Fixture.Build<T>().OmitAutoProperties().Create();
        }

        public virtual void Setup()
        {

        }

        public void Dispose()
        {
            ((IDisposable)Connection).Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
