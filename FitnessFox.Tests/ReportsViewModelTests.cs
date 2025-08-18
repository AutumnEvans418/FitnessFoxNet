using AutoFixture;
using FitnessFox.Components.ViewModels;
using FitnessFox.Data.Vitals;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Tests
{
    public class ReportsViewModelTests : DbTestBase<ReportsViewModel>
    {

        [Fact]
        public async Task Refresh_Should_SetSeries()
        {
            await Subject.Refresh();

            Subject.WeightSeries.Count.Should().Be(1);
            Subject.WeightSeries[0].Data.Should().HaveCount(7);
        }

        [Fact]
        public async Task Monthly_Should_Have7()
        {
            var user = Db.Users.First();

            var vitals = Fixture.Build<UserVital>()
                .With(w => w.UserId, user.Id)
                .Without(w => w.User)
                .With(w => w.Type, UserVitalType.Weight)
                .CreateMany(10);

            Db.UserVitals.AddRange(vitals);
            Db.SaveChanges();

            Subject.From = Subject.To.GetValueOrDefault().AddDays(-30);

            await Subject.Refresh();

            Subject.WeightSeries[0].Data.Should().HaveCount(7);
        }

        [Fact]
        public async Task Weekly_Should_StartAtFrom()
        {
            Subject.From = DateTime.Parse("2025-01-01");
            Subject.To = DateTime.Parse("2025-01-07");

            await Subject.Refresh();

            Subject.Labels.Should().HaveCount(7);
            Subject.WeightSeries[0].Data.Should().HaveCount(7);

            Subject.Labels.First().Should().Be("Jan 1");
            Subject.Labels.Last().Should().Be("Jan 7");
        }

        [Fact]
        public async Task Weekly_Should_HaveData()
        {
            Subject.From = DateTime.Parse("2025-01-01");
            Subject.To = DateTime.Parse("2025-01-07");

            var user = Db.Users.First();
            for (int i = 0; i < 7; i++)
            {
                var date = Subject.From.GetValueOrDefault().AddDays(i);

                Db.UserVitals.Add(new UserVital
                {
                    Date = date,
                    Type = UserVitalType.Weight,
                    UserId = user.Id,
                    Value = 1,
                });
            }

            Db.SaveChanges();

            await Subject.Refresh();

            Subject.WeightSeries.First().Data.Average().Should().Be(1);
        }

        [Fact]
        public async Task BiWeekly_Should_HaveAverageData()
        {
            Subject.From = DateTime.Parse("2025-01-01");
            Subject.To = DateTime.Parse("2025-01-14");

            var user = Db.Users.First();
            for (int i = 0; i < 7; i++)
            {
                var date = Subject.From.GetValueOrDefault().AddDays(i * 2);

                Db.UserVitals.Add(new UserVital
                {
                    Date = date,
                    Type = UserVitalType.Weight,
                    UserId = user.Id,
                    Value = 3,
                });

                var date1 = date.AddDays(1);

                Db.UserVitals.Add(new UserVital
                {
                    Date = date1,
                    Type = UserVitalType.Weight,
                    UserId = user.Id,
                    Value = 7,
                });
            }

            Db.SaveChanges();

            await Subject.Refresh();

            Subject.WeightSeries.First().Data.Average().Should().Be(5);
        }

        [Fact]
        public async Task GoalWeight_Should_CreateSeries()
        {
            var user = Db.Users.First();

            Db.UserGoals.Add(new Data.Goals.UserGoal
            {
                UserId = user.Id,
                Type = Data.Goals.UserGoalType.WeightLbs,
                Value = 10,
            });

            Db.SaveChanges();

            await Subject.Refresh();

            Subject.WeightSeries.Should().HaveCount(2);

            Subject.WeightSeries.Last().Data.Average().Should().Be(10);

        }
    }
}
