using AutoFixture;
using FitnessFox.Components.ViewModels;
using FitnessFox.Data.Foods;
using FitnessFox.Data.Vitals;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FitnessFox.Tests.ViewModels
{
    public class ReportsViewModelTests : DbTestBase<ReportsViewModel>
    {
        public ReportsViewModelTests()
        {
            Subject.From = DateTime.Parse("2025-01-01");
            Subject.To = DateTime.Parse("2025-01-14");
        }

        [Fact]
        public async Task Refresh_Series_Should_BeEmpty()
        {
            await Subject.Refresh();

            Subject.Charts[0].Series.Count.Should().Be(1);
        }

        [Fact]
        public async Task Monthly_Should_Have1()
        {
            var user = Db.Users.First();

            var vitals = Fixture.Build<UserVital>()
                .With(w => w.UserId, user.Id)
                .Without(w => w.User)
                .With(w => w.Type, UserVitalType.Weight)
                .With(w => w.Date, Subject.From.GetValueOrDefault())
                .CreateMany(10);

            Db.UserVitals.AddRange(vitals);
            Db.SaveChanges();

            Subject.From = Subject.To.GetValueOrDefault().AddDays(-30);

            await Subject.Refresh();

            Subject.Charts[0].Series[0].Data.Should().HaveCount(1);
        }

        [Fact]
        public async Task Weekly_Should_StartAtFrom()
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

            Subject.Charts[0].Labels.Should().HaveCount(7);
            Subject.Charts[0].Series[0].Data.Should().HaveCount(7);

            Subject.Charts[0].Labels.First().Should().Be("Jan 1");
            Subject.Charts[0].Labels.Last().Should().Be("Jan 7");
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

            Subject.Charts[0].Series.First().Data.Average().Should().Be(1);
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

            Subject.Charts[0].Series.First().Data.Average().Should().Be(5);
        }

        [Fact]
        public async Task GoalWeight_Should_CreateSeries()
        {
            Subject.From = DateTime.Parse("2025-01-01");
            Subject.To = DateTime.Parse("2025-01-14");

            var user = Db.Users.First();

            Db.UserGoals.Add(new Data.Goals.UserGoal
            {
                UserId = user.Id,
                Type = Data.Goals.UserGoalType.WeightLbs,
                Value = 10,
            });

            Db.UserVitals.Add(new UserVital
            {
                Date = Subject.From.GetValueOrDefault(),
                Type = UserVitalType.Weight,
                UserId = user.Id,
                Value = 3,
            });

            Db.SaveChanges();

            await Subject.Refresh();

            Subject.Charts[0].Series.Should().HaveCount(2);

            Subject.Charts[0].Series.Last().Data.Average().Should().Be(10);

        }

        [Fact]
        public async Task Calories_Should_BeOnChart()
        {
            var user = Db.Users.First();

            Db.UserMeals.Add(new UserMeal()
            {
                Calories = 10,
                Date = Subject.From.GetValueOrDefault(),
                UserId = user.Id,
            });

            Db.SaveChanges();

            await Subject.Refresh();

            var chart = Subject.Charts.First(p => p.Name == ChartDataType.Food);
            
            chart.Labels.Should().HaveCount(1);
            chart.Series.First().Data.Average().Should().Be(10);
        }
    }
}
