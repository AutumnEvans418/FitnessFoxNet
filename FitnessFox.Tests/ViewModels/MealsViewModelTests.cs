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
    public class MealsViewModelTests : DbTestBase<MealsViewModel>
    {
        public MealsViewModelTests()
        {
            var seed = Fixture.Create<FakeDataGenerator>();
            seed.Seed = 1;
            seed.SeedData().Wait();
        }

        [Fact]
        public async Task Initialized_UserMeals_Should_NotBeEmpty()
        {
            await Subject.OnInitializedAsync();

            Subject.UserMeals.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Search_Empty_Should_ReturnTop40()
        {
            await Subject.OnInitializedAsync();

            var result = await Subject.Search("", CancellationToken.None);
            
            result.Count().Should().Be(40);
        }

        [Fact]
        public async Task Search_PartialMatch_Should_ReturnMatching()
        {
            var food = Db.Foods.First().Description.Substring(0,5);

            await Subject.OnInitializedAsync();

            var result = await Subject.Search(food, CancellationToken.None);

            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Search_Match_Should_ReturnMatching()
        {
            var food = Db.Foods.First().Description.ToUpper();

            await Subject.OnInitializedAsync();

            var result = await Subject.Search(food, CancellationToken.None);

            result.Count().Should().Be(1);
        }
    }
}
