using AutoFixture;
using FitnessFox.Components.Services;
using FitnessFox.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Tests
{
    public class SettingsServiceTests
    {
        public SqliteConnection Connection { get; set; }
        public Fixture Fixture { get; set; }

        public SettingsService SettingsService { get; set; }
        public IAuthenticationService AuthenticationService { get; set; }
        public ApplicationDbContext Db { get; set; }
        public SettingsServiceTests()
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
            SettingsService = Fixture.Build<SettingsService>().OmitAutoProperties().Create();
        }

        [Fact]
        public void Db_Should_HaveNoSettings()
        {
            Db.UserSettings.Count().Should().Be(0);
        }

        [Fact]
        public async Task SetSettingString_ShouldReturn()
        {
            var guid = Guid.NewGuid().ToString();
            await SettingsService.SetValue(Components.Data.Settings.SettingKey.SpreadsheetId, guid);

            Db.UserSettings.Count().Should().Be(1);
        }

        [Fact]
        public async Task GetSettingString_ShouldReturn()
        {
            var guid = Guid.NewGuid().ToString();
            await SettingsService.SetValue(Components.Data.Settings.SettingKey.SpreadsheetId, guid);

            var str = await SettingsService.GetValue<string?>(Components.Data.Settings.SettingKey.SpreadsheetId);

            str.Should().Be(guid);
        }

        [Fact]
        public async Task GetSettingDate_ShouldReturn()
        {
            var guid = DateTime.Now;
            await SettingsService.SetValue(Components.Data.Settings.SettingKey.SpreadsheetId, guid);

            var str = await SettingsService.GetValue<DateTime?>(Components.Data.Settings.SettingKey.SpreadsheetId);

            str.Should().Be(guid);
        }

        [Fact]
        public async Task GetSettingBool_ShouldReturn()
        {
            var guid = true;
            await SettingsService.SetValue(Components.Data.Settings.SettingKey.SpreadsheetId, guid);

            var str = await SettingsService.GetValue<bool?>(Components.Data.Settings.SettingKey.SpreadsheetId);

            str.Should().Be(guid);
        }
    }
}
