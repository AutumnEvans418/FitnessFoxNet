using AutoFixture;
using CsvHelper;
using CsvHelper.Configuration;
using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Globalization;
using Xunit;

namespace FitnessFox.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CsvShouldCreateHeader()
        {
            using var stream = new MemoryStream();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture);

            config.IgnoreReferences = true;

            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(new List<Food>());
            }

            var result = System.Text.Encoding.UTF8.GetString(stream.ToArray());

            result.Should().StartWith("Id");
            result.Should().NotContain("UserName");
            result.Should().NotContain("User,");
        }
    }

    public class GoogleSyncServiceTests
    {
        GoogleSyncService GoogleSyncService { get; set; }
        public GoogleSyncServiceTests()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoFixture.AutoNSubstitute.AutoNSubstituteCustomization());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite("Data Source=:memory:").Options;

            var db = new ApplicationDbContext(options);

            db.Database.EnsureCreated();
            fixture.Inject(db);

            var file = fixture.Freeze<IFileService>();

            var stream = File.OpenRead("..\\..\\..\\..\\FitnessFox.Mobile\\Resources\\Raw\\fitnessfox-467923-9857f7a3ab7e.json");

            file.GetLocalFileAsync(Arg.Any<string>()).Returns(stream);

            fixture.Inject(Options.Create(new GoogleOptions()));

            GoogleSyncService = fixture.Build<GoogleSyncService>().OmitAutoProperties().Create();
        }

        [Fact]
        public async Task GetData()
        {
            var service = await GoogleSyncService.GetSheetService();
            var sheet = await GoogleSyncService.GetSheet(service);

            var result = await GoogleSyncService.GetData<Food>(sheet);

            result.Should().NotBeEmpty();
        }
    }
}