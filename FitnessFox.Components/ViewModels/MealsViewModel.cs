using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using FitnessFox.Data.Goals;
using FitnessFox.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FitnessFox.Components.ViewModels
{
    public class MealsViewModel : ViewModelBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly ApplicationDbContext dbContext;

        public MealsViewModel(
            IDialogService mudDialogInstance, 
            ILoggingService loggingService, 
            ILoadingService loadingService,
            IAuthenticationService authenticationService,
            ApplicationDbContext dbContext) 
            : base(mudDialogInstance, loggingService, loadingService)
        {
            this.authenticationService = authenticationService;
            this.dbContext = dbContext;
        }

        public DateTime? CurrentDate { get; set; } = DateTime.Now;
        public ApplicationUser User { get; set; } = new();
        public List<UserMeal> UserMeals { get; set; } = new();
        public List<UserGoal> Goals { get; set; } = [];

        public override async Task OnInitializedAsync()
        {
            await Load(Refresh);
        }

        public async Task Refresh()
        {

            var user = await authenticationService.GetUserAsync();

            if (user == null)
                return;

            User = user;

            var currentDate = CurrentDate.GetValueOrDefault().Date.ToDateOnly();

            Goals = await dbContext.UserGoals.Where(u => u.UserId == user.Id).ToListAsync();

            UserMeals = await dbContext
                .UserMeals
                .Where(u => u.UserId == user.Id && u.Date == currentDate)
                .Include(u => u.Food)
                .Include(u => u.Recipe)
                .OrderBy(p => p.Type)
                .ToListAsync();
        }

        public async Task SetDate(DateTime? date)
        {
            CurrentDate = date;
            await Refresh();
        }

        public void Add(UserMealType type)
        {
            var meal = new UserMeal
            {
                Date = CurrentDate.GetValueOrDefault().ToDateOnly(),
                UserId = User.Id,
                Type = type,
            };
            UserMeals.Add(meal);
        }
        public async Task<IEnumerable<Nutrients>> Search(string value, CancellationToken token)
        {
            var foods = dbContext.Foods.OrderBy(f => f.BrandRestaurant).AsQueryable();
            var recipes = dbContext.Recipes.OrderBy(f => f.Name).AsQueryable();

            if (!string.IsNullOrWhiteSpace(value))
            {
                var v = value.ToLower();

                foods = foods.Where(f => 
                    f.BrandRestaurant.ToLower().Contains(v) || 
                    f.Description.ToLower().Contains(v));
                recipes = recipes.Where(f => f.Name.ToLower().Contains(v));
            }

            var foodsList = await foods.Take(20).ToListAsync();
            var recipesList = await recipes.Take(20).ToListAsync();

            return foodsList
                .Cast<Nutrients>()
                .Union(recipesList)
                .OrderBy(n =>
                {
                    if (n is Food f)
                        return f.Name;
                    if (n is Recipe r)
                        return r.Name;
                    throw new NotImplementedException();
                }).ToList();
        }

        public async Task Update(UserMeal meal)
        {
            meal.SetNutrients();

            dbContext.Update(meal);
            await dbContext.SaveChangesAsync();
        }
    }
}
