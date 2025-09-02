using AutoFixture;
using FitnessFox.Components.Services;
using FitnessFox.Components.ViewModels;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Tests.ViewModels
{
    public class HomeViewModelSeededTests : DbTestBase<HomeViewModel>
    {
        public HomeViewModelSeededTests()
        {
            var seed = Fixture.Create<FakeDataGenerator>();
            seed.Seed = 1;
            seed.SeedData().Wait();
        }

        [Fact]
        public async Task Initialized_Should_SetCurrentDate()
        {
            await Subject.OnInitializedAsync();
            Subject.CurrentDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Initialized_Should_SetWeight()
        {
            await Subject.OnInitializedAsync();
            Subject.Weight.Should().NotBeNull();
        }

        [Fact]
        public async Task StandardVitals_Should_HaveCount()
        {
            await Subject.OnInitializedAsync();
            Subject.BasicVitalsDisplay.Should().HaveCount(6);
        }

        [Fact]
        public async Task ChangeDate_StandardVitals_Should_HaveCount()
        {
            await Subject.OnInitializedAsync();
            await Subject.SetDate(Subject.CurrentDate.Value.AddDays(-1));
            Subject.BasicVitalsDisplay.Should().HaveCount(6);
        }
    }

    public class HomeViewModelTests : DbTestBase<HomeViewModel>
    {
        [Fact]
        public void Db_Should_HaveNoWeight()
        {
            Db.UserVitals.Should().BeEmpty();
        }

        [Fact]
        public async Task Save_Should_AddWeight()
        {
            await Subject.OnInitializedAsync();

            Subject.Weight.Value += 1;

            await Subject.Save(Subject.Weight);

            Db.UserVitals.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Save_Should_UpdateWeight()
        {
            var user = await AuthenticationService.GetUserAsync();
            Db.UserVitals.Add(new Data.Vitals.UserVital
            {
                User = user!,
                Type = Data.Vitals.UserVitalType.Weight,
                Date = DateOnly.FromDateTime(Subject.CurrentDate.GetValueOrDefault()),
            });
            Db.SaveChanges();

            await Subject.OnInitializedAsync();

            Subject.Weight.Value += 1;

            await Subject.Save(Subject.Weight);

            Db.UserVitals.First().Value.Should().Be(1);
        }
    }
}
