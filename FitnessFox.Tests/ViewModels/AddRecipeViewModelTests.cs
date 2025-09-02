using AutoFixture;
using FitnessFox.Components.Services;
using FitnessFox.Components.ViewModels.Foods;
using FluentAssertions;

namespace FitnessFox.Tests.ViewModels
{
    public class AddRecipeViewModelTests : DbTestBase<AddRecipeViewModel>
    {
        public AddRecipeViewModelTests()
        {
            var seed = Fixture.Create<FakeDataGenerator>();
            seed.Seed = 1;
            seed.SeedData().Wait();
        }

        [Fact]
        public async Task Initialized_Model_Should_NotBeNull()
        {
            Subject.Id = Db.Recipes.First().Id.ToString();
            await Subject.OnInitializedAsync();
            Subject.Model.Should().NotBeNull();
        }
    }
}
