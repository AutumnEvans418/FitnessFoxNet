using AutoFixture;
using FitnessFox.Components.Data.Options;
using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using FitnessFox.Data.Goals;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace FitnessFox.Tests
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
            var userId = await AuthenticationService.GetUserAsync();
            List<List<string>> data = [
                ["Id", "UserId"],
                [Guid.NewGuid().ToString(), userId.Id]
                ];

            GoogleSheetsServices.GetSheetRows("Food").Returns(data);

            var result = await Subject.GetData<Food>();

            result.Should().NotBeEmpty();
        }

        [Fact]
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

            await Subject.Sync();

            await GoogleSheetsServices.Received(1).UpdateSheet(Arg.Is<string>(s => s.StartsWith("Food")), Arg.Is<IList<IList<object>>>(o => o.Count == 2));
        }

        [Fact]
        public async Task GetEnums()
        {
            var userId = await AuthenticationService.GetUserAsync();
            List<List<string>> data = [
                ["Id", "UserId", "Type"],
                [Guid.NewGuid().ToString(), userId.Id, ((int)(UserGoalType.Bmi)).ToString()],
                [Guid.NewGuid().ToString(), userId.Id, (UserGoalType.VitaminK).ToString()]
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