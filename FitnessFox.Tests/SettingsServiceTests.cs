using FitnessFox.Components.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Tests
{

    public class SettingsServiceTests : DbTestBase<SettingsService>
    {
        [Fact]
        public void Db_Should_HaveNoSettings()
        {
            Db.UserSettings.Count().Should().Be(0);
        }

        [Fact]
        public async Task SetSettingString_ShouldReturn()
        {
            var guid = Guid.NewGuid().ToString();
            await Subject.SetValue(Components.Data.Settings.SettingKey.SpreadsheetId, guid);

            Db.UserSettings.Count().Should().Be(1);
        }

        [Fact]
        public async Task GetSettingString_ShouldReturn()
        {
            var guid = Guid.NewGuid().ToString();
            await Subject.SetValue(Components.Data.Settings.SettingKey.SpreadsheetId, guid);

            var str = await Subject.GetValue<string?>(Components.Data.Settings.SettingKey.SpreadsheetId);

            str.Should().Be(guid);
        }

        [Fact]
        public async Task GetSettingDate_ShouldReturn()
        {
            var guid = DateTime.Now;
            await Subject.SetValue(Components.Data.Settings.SettingKey.SpreadsheetId, guid);

            var str = await Subject.GetValue<DateTime?>(Components.Data.Settings.SettingKey.SpreadsheetId);

            str.Should().Be(guid);
        }

        [Fact]
        public async Task GetSettingBool_ShouldReturn()
        {
            var guid = true;
            await Subject.SetValue(Components.Data.Settings.SettingKey.SpreadsheetId, guid);

            var str = await Subject.GetValue<bool?>(Components.Data.Settings.SettingKey.SpreadsheetId);

            str.Should().Be(guid);
        }
    }
}
