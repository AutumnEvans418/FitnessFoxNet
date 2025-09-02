using Bogus;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using FitnessFox.Data.Vitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace FitnessFox.Components.Services
{
    public class FakeDataGenerator : IFakeDataGenerator
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IAuthenticationService authenticationService;

        public FakeDataGenerator(
            ApplicationDbContext applicationDbContext,
            IAuthenticationService authenticationService)
        {
            this.applicationDbContext = applicationDbContext;
            this.authenticationService = authenticationService;
        }

        public int Seed { get; set; }

        public async Task SeedData()
        {
            var user = await authenticationService.GetUserAsync();

            if (user == null)
                return;

            var date = DateTime.Now;

            var units = new[] { "Unit", "tsp", "Tbsp", "Cup", "Item" };

            var foodGen = new Faker<Food>()
                .StrictMode(true)
                .RuleFor(f => f.UserId, f => user?.Id)
                .RuleFor(f => f.User, f => null)
                .RuleFor(f => f.Id, f => Guid.Empty)
                .RuleFor(f => f.DateCreated, f => DateTime.Now)
                .RuleFor(f => f.DateModified, f => DateTime.Now)
                .RuleFor(f => f.BrandRestaurant, f => f.Company.CompanyName())
                .RuleFor(f => f.Description, f => f.Commerce.ProductName())
                .RuleFor(f => f.ServingSize, f => f.Random.Float(0.5f, 5f))
                .RuleFor(f => f.ServingUnit, f => f.PickRandom(units))
                .RuleFor(f => f.TotalServings, f => f.Random.Float(1, 10))
                .RuleFor(f => f.Calories, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.TotalFat, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.SaturatedFat, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.PolysaturatedFat, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.MonosaturatedFat, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.TransFat, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.Cholesterol, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.Sodium, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.Potassium, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.Carbs, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.Fiber, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.Sugar, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.Protein, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.VitaminA, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.VitaminC, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.Calcium, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.Iron, f => f.Random.Float(0, 2000))
                .RuleFor(f => f.VitaminK, f => f.Random.Float(0, 2000))
                .UseSeed(Seed)
                ;

            var foods = foodGen.Generate(20).ToList();

            applicationDbContext.Foods.AddRange(foods);

            var recipeFoodGen = new Faker<RecipeFood>()
                .RuleFor(f => f.Id, f => Guid.Empty)
                .RuleFor(f => f.RecipeId, f => Guid.Empty)
                .RuleFor(f => f.FoodId, f => Guid.Empty)
                .RuleFor(f => f.Amount, f => f.Random.Float(0.5f, 10))
                .RuleFor(f => f.Recipe, f => null)
                .RuleFor(f => f.Food, f => f.PickRandom(foods))
                .RuleFor(f => f.DateCreated, f => DateTime.Now)
                .RuleFor(f => f.DateModified, f => DateTime.Now)
                .UseSeed(Seed)
                .StrictMode(true);

            var recipeGen = new Faker<Recipe>()
                .RuleFor(f => f.Id, f => Guid.Empty)
                .RuleFor(f => f.DateCreated, f => DateTime.Now)
                .RuleFor(f => f.DateModified, f => DateTime.Now)
                .RuleFor(f => f.Name, f => f.Commerce.ProductName())
                .RuleFor(f => f.NumberOfPeople, f => f.Random.Int(1, 10))
                .RuleFor(f => f.UserId, f => user?.Id)
                .RuleFor(f => f.User, f => null)
                .RuleFor(f => f.Foods, f => recipeFoodGen.GenerateBetween(1, 10).ToList())
                .FinishWith((f, a) => a.SetNutrients())
                .UseSeed(Seed)
                .StrictMode(false);

            var recipes = recipeGen.Generate(20).ToList();

            applicationDbContext.Recipes.AddRange(recipes);

            var userMealGen = new Faker<UserMeal>()
                .RuleFor(f => f.Id, f => Guid.Empty)
                .RuleFor(f => f.DateCreated, f => DateTime.Now)
                .RuleFor(f => f.DateModified, f => DateTime.Now)
                .RuleFor(f => f.Date, f => f.Date.Between(DateTime.Now.AddDays(-7), date))
                .RuleFor(f => f.UserId, f => user?.Id)
                .RuleFor(f => f.User, f => null)
                .RuleFor(f => f.Type, f => f.PickRandom<UserMealType>())
                .RuleFor(f => f.Servings, f => f.Random.Int(1, 10))
                .RuleFor(f => f.MealItem, f => f.PickRandom(foods.OfType<Nutrients>().Union(recipes)))
                .FinishWith((f, a) => a.SetNutrients())
                .UseSeed(Seed)


                .StrictMode(false);

            var userMeals = userMealGen.Generate(20).ToList();

            applicationDbContext.UserMeals.AddRange(userMeals);

            var userVitalGen = new Faker<UserVital>()
                .RuleFor(f => f.Id, f => Guid.Empty)
                .RuleFor(f => f.UserId, f => user?.Id)
                .RuleFor(f => f.User, f => null)
                .RuleFor(f => f.Date, f => DateOnly.FromDateTime(f.Date.Between(DateTime.Now.AddDays(-7), date)))
                .RuleFor(f => f.DateCreated, f => DateTime.Now)
                .RuleFor(f => f.DateModified, f => DateTime.Now)
                .RuleFor(f => f.Type, f => f.PickRandom<UserVitalType>())
                .RuleFor(f => f.Value, f => f.Random.Float(1, 200))
                .UseSeed(Seed)

                .StrictMode(true);

            var vitals = userVitalGen.Generate(20).ToList();

            applicationDbContext.UserVitals.AddRange(vitals);

            await this.applicationDbContext.SaveChangesAsync();
        }

    }
}
