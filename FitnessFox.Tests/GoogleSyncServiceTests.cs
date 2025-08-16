using AutoFixture;
using FitnessFox.Components.Data.Options;
using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace FitnessFox.Tests
{
    
    public class GoogleSyncServiceTests : IDisposable
    {
        GoogleSyncService GoogleSyncService { get; set; }
        DbContextOptions<ApplicationDbContext> DbOptions { get; set; }
        public ApplicationDbContext Db { get; set; }
        public Fixture Fixture { get; set; }
        public SqliteConnection Connection { get; set; }
        public GoogleSyncServiceTests()
        {
            Connection = new SqliteConnection("Filename=:memory:");
            Connection.Open();
            var fixture = new Fixture();
            fixture.Customize(new AutoFixture.AutoNSubstitute.AutoNSubstituteCustomization());
            Fixture = fixture;
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(Connection).Options;
            DbOptions = options;
            var db = new ApplicationDbContext(options);
            Db = db;
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            fixture.Inject(db);

            var file = fixture.Freeze<IFileService>();

            var stream = File.OpenRead("..\\..\\..\\..\\FitnessFox.Mobile\\Resources\\Raw\\fitnessfox-467923-9857f7a3ab7e.json");

            file.GetLocalFileAsync(Arg.Any<string>()).Returns(stream);

            fixture.Inject(Options.Create(new GoogleOptions()));

            GoogleSyncService = fixture.Build<GoogleSyncService>().OmitAutoProperties().Create();
        }

        [Fact(Skip = "test")]
        public async Task GetData()
        {
            var service = await GoogleSyncService.GetSheetService();
            var sheet = await GoogleSyncService.GetSheet(service);

            var result = await GoogleSyncService.GetData<Food>(sheet);

            result.Should().NotBeEmpty();
        }

        [Fact(Skip = "test2")]
        public async Task SyncData()
        {
            var db = Db;

            var user = new ApplicationUser();
            user.Id = "26218fa1-7163-4b42-aa57-51ef66515203";
            db.Users.Add(user);

            db.Foods.Add(Fixture.Build<Food>()
                .Without(p => p.User)
                .With(p => p.UserId, user.Id)
                .Create());

            db.SaveChanges();

            await GoogleSyncService.Sync();
        }

        public void Dispose()
        {
            ((IDisposable)Connection).Dispose();
        }
    }
}