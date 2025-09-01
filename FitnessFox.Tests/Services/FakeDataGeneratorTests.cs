using FitnessFox.Components.Services;
using FluentAssertions;

namespace FitnessFox.Tests.Services
{
    public class FakeDataGeneratorTests : DbTestBase<FakeDataGenerator>
    {
        [Fact]
        public async Task SeedData_Should_CreateFoods()
        {
            await Subject.SeedData();

            Db.Foods.Should().HaveCount(20);
        }

        [Fact]
        public async Task SeedData_Should_CreateRecipes()
        {
            await Subject.SeedData();

            Db.Recipes.Should().HaveCount(20);
            Db.RecipeFoods.Should().NotBeEmpty();
            Db.Recipes.First().Calories.Should().NotBe(0);
        }

        [Fact]
        public async Task SeedData_Should_CreateMeals()
        {
            await Subject.SeedData();

            Db.UserMeals.Should().HaveCount(20);
            Db.UserMeals.First().Calories.Should().NotBe(0);
        }

        [Fact]
        public async Task SeedData_Should_CreateVitals()
        {
            await Subject.SeedData();

            Db.UserVitals.Should().HaveCount(20);
        }
    }
}