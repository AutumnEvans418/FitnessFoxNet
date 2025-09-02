using AutoFixture;
using FitnessFox.Components.Services;
using FitnessFox.Components.ViewModels.Foods;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Tests.ViewModels
{

    public class AddFoodViewModelTests : DbTestBase<AddFoodViewModel>
    {
        public AddFoodViewModelTests()
        {
            var seed = Fixture.Create<FakeDataGenerator>();
            seed.Seed = 1;
            seed.SeedData().Wait();
        }

        [Fact]
        public async Task Initialized_Units_Should_NotBeEmpty()
        {
            Subject.Id = Db.Foods.First().Id.ToString();
            await Subject.OnInitializedAsync();
            Subject.Units.Should().NotBeEmpty();
        }
    }
}
