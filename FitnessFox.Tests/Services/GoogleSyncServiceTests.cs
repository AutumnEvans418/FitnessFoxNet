using AutoFixture;
using FitnessFox.Components.Data.Options;
using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using FitnessFox.Data.Goals;
using FitnessFox.Data.Vitals;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace FitnessFox.Tests.Services
{
    public class GoogleSyncServiceTests : DbTestBase<GoogleSyncService>
    {

        IGoogleSheetsServices GoogleSheetsServices { get; set; }

        public override void Setup()
        {
            GoogleSheetsServices = Fixture.Freeze<IGoogleSheetsServices>();

            base.Setup();
        }

        [Fact]
        public async Task GetData()
        {
            var user = await AuthenticationService.GetUserAsync();
            List<List<string>> data = [
                ["Id", "UserId"],
                [Guid.NewGuid().ToString(), user.Id]
                ];

            GoogleSheetsServices.GetSheetRows(nameof(Food)).Returns(data);

            var result = await Subject.GetData<Food>();

            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task SyncData_LoggedInUser_ShouldSyncOnly()
        {
            var db = Db;

            var user = await AuthenticationService.GetUserAsync();

            var otherUser = Fixture.Create<ApplicationUser>();

            db.Users.Add(otherUser);

            var vitals = Fixture.Build<UserVital>()
                .Without(p => p.User)
                .With(p => p.UserId, user.Id)
                .CreateMany(10);

            var otherUservitals = Fixture.Build<UserVital>()
                .Without(p => p.User)
                .With(p => p.UserId, otherUser.Id)
                .CreateMany(10);

            db.UserVitals.AddRange(vitals);
            db.UserVitals.AddRange(otherUservitals);
            await db.SaveChangesAsync();

            await Subject.Sync();

            await GoogleSheetsServices.Received(1).UpdateSheet(
                Arg.Is<string>(s => s.StartsWith(nameof(UserVital))), 
                Arg.Is<IList<IList<object>>>(l => l.Count == 11));
        }

        [Fact]
        public async Task Users_Should_BeOne()
        {
            Db.Users.Should().HaveCount(1);
        }

        [Fact]
        public async Task SyncDataTwice_Vital_LoggedInUser_ShouldNotFail()
        {
            var db = Db;

            var user = await AuthenticationService.GetUserAsync();

            var vitals = Fixture.Build<UserVital>()
                .Without(p => p.User)
                .With(p => p.UserId, user.Id)
                .CreateMany(10);

            Db.AddRange(vitals);
            Db.SaveChanges();

            List<List<string>> data = [
               ["Id", "UserId"],
               ];

            foreach (var item in vitals)
            {
                data.Add([item.Id.ToString(), user.Id]);
                data.Add([Guid.NewGuid().ToString(), Guid.NewGuid().ToString()]);
            }

            GoogleSheetsServices.GetSheetRows(nameof(UserVital)).Returns(data);

            await Subject.Sync();
            await Subject.Sync();

            Db.UserVitals.Should().HaveCount(20);
        }

        [Fact]
        public async Task SyncData_Sheets_Should_ExcludeMissingIds()
        {
            var db = Db;

            var user = await AuthenticationService.GetUserAsync();

            List<List<string>> data = [
                ["Id", "UserId"],
                [Guid.NewGuid().ToString(), user.Id],
                ["", ""],
                [null, null]
                ];

            GoogleSheetsServices.GetSheetRows(nameof(UserVital)).Returns(data);


            await Subject.Sync();

            db.UserVitals.Should().HaveCount(1);
        }

        [Fact]
        public async Task Sync_MatchingData_ShouldNotChange()
        {
            var db = Db;

            var user = await AuthenticationService.GetUserAsync();

            var vital = Db.UserVitals.Add(Fixture.Build<UserVital>()
                .Without(p => p.User)
                .With(p => p.UserId, user.Id)
                .Create());

            Db.SaveChanges();

            List<List<string>> data = [
                [nameof(UserVital.Id), nameof(UserVital.UserId), nameof(UserVital.DateModified)],
                [vital.Entity.Id.ToString(), user.Id, vital.Entity.DateModified.ToString()],
                ];

            GoogleSheetsServices.GetSheetRows(nameof(UserVital)).Returns(data);

            await Subject.SyncDbSet<UserVital, Guid>(p => true, p => { });

            db.UserVitals.Should().HaveCount(1);
            await GoogleSheetsServices.DidNotReceive().UpdateSheet(Arg.Is<string>(s => s.StartsWith(nameof(UserVital))), Arg.Any<IList<IList<object>>>());
        }

        [Fact]
        public async Task Sync_ExistingDataUpdatedInSheets_ShouldChangeDb()
        {
            var db = Db;

            var user = await AuthenticationService.GetUserAsync();

            var vital = Db.UserVitals.Add(Fixture.Build<UserVital>()
                .Without(p => p.User)
                .With(p => p.UserId, user.Id)
                .Create());

            Db.SaveChanges();

            var value = vital.Entity.Value + 10;

            List<List<string>> data = [
                [nameof(UserVital.Id), nameof(UserVital.UserId), nameof(UserVital.DateModified), nameof(UserVital.Value)],
                [vital.Entity.Id.ToString(), user.Id, vital.Entity.DateModified.AddDays(1).ToString(), (value).ToString()],
                ];

            GoogleSheetsServices.GetSheetRows(nameof(UserVital)).Returns(data);

            await Subject.SyncDbSet<UserVital, Guid>(p => true, p => { });

            db.UserVitals.Should().HaveCount(1);

            db.UserVitals.First().Value.Should().Be(value);

            await GoogleSheetsServices.DidNotReceive().UpdateSheet(Arg.Is<string>(s => s.StartsWith(nameof(UserVital))), Arg.Any<IList<IList<object>>>());
        }

        [Fact]
        public async Task Sync_ExistingDataUpdatedInDb_ShouldChangeSheets()
        {
            var db = Db;

            var user = await AuthenticationService.GetUserAsync();

            var vital = Db.UserVitals.Add(Fixture.Build<UserVital>()
                .Without(p => p.User)
                .With(p => p.UserId, user.Id)
                .Create());

            Db.SaveChanges();

            List<List<string>> data = [
                [nameof(UserVital.Id), nameof(UserVital.UserId), nameof(UserVital.DateModified), nameof(UserVital.Value)],
                [vital.Entity.Id.ToString(), user.Id, vital.Entity.DateModified.AddDays(-1).ToString(), (vital.Entity.Value + 10).ToString()],
                ];

            GoogleSheetsServices.GetSheetRows(nameof(UserVital)).Returns(data);


            await Subject.Sync();

            db.UserVitals.Should().HaveCount(1);

            db.UserVitals.First().Value.Should().Be(vital.Entity.Value);

            await GoogleSheetsServices.Received(1).UpdateSheet(Arg.Is<string>(s => s.StartsWith(nameof(UserVital))), Arg.Any<IList<IList<object>>>());
        }

        [Fact]
        public async Task SyncData_Db_ShouldUpdateSheet()
        {
            var db = Db;

            var user = await AuthenticationService.GetUserAsync();

            db.Foods.Add(Fixture.Build<Food>()
                .Without(p => p.User)
                .With(p => p.UserId, user.Id)
                .Create());

            db.SaveChanges();

            await Subject.Sync();

            await GoogleSheetsServices.Received(1).UpdateSheet(Arg.Is<string>(s => s.StartsWith("Food")), Arg.Is<IList<IList<object>>>(o => o.Count == 2));
        }

        [Fact]
        public async Task GetEnums()
        {
            var userId = await AuthenticationService.GetUserAsync();
            List<List<string>> data = [
                ["Id", "UserId", "Type"],
                [Guid.NewGuid().ToString(), userId.Id, ((int)UserGoalType.Bmi).ToString()],
                [Guid.NewGuid().ToString(), userId.Id, UserGoalType.VitaminK.ToString()]
                ];

            GoogleSheetsServices.GetSheetRows(nameof(UserGoal)).Returns(data);

            var result = await Subject.GetData<UserGoal>();

            result.Should().HaveCount(2);
            result[0].Type.Should().Be(UserGoalType.Bmi);
            result[1].Type.Should().Be(UserGoalType.VitaminK);
        }

        [Fact]
        public async Task UpdateEnumsAsString()
        {
            var user = await AuthenticationService.GetUserAsync();

            Db.UserGoals.AddRange(Fixture
                .Build<UserGoal>()
                .Without(u => u.User)
                .With(u => u.UserId, user.Id)
                .With(u => u.Type, UserGoalType.Bmi)
                .CreateMany(3));
            Db.SaveChanges();

            await Subject.Sync();

            await GoogleSheetsServices
                .Received(1)
                .UpdateSheet(Arg.Is<string>(s => s.StartsWith(nameof(UserGoal))), Arg.Is<IList<IList<object>>>(o => o.Count == 4 && o[1][3].ToString() == UserGoalType.Bmi.ToString()));

        }
    }
}