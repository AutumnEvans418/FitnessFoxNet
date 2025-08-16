using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.ViewModels.Foods
{
    public class AddRecipeViewModel : ViewModelBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IJSRuntime jsRuntime;
        private readonly ApplicationDbContext dbContext;

        public AddRecipeViewModel(
            IDialogService mudDialogInstance, 
            ILoggingService loggingService, 
            ILoadingService loadingService,
            IAuthenticationService authenticationService,
            IJSRuntime jsRuntime,
            ApplicationDbContext dbContext) : base(mudDialogInstance, loggingService, loadingService)
        {
            this.authenticationService = authenticationService;
            this.jsRuntime = jsRuntime;
            this.dbContext = dbContext;
        }
        public Recipe? Model { get; set; }

        public int? Id { get; set; }
        public override async Task OnInitializedAsync()
        {
            await Load(Refresh);
        }

        private async Task Refresh()
        {
            if (Id != null)
            {
                Model = await dbContext
                    .Recipes
                    .Include(r => r.Foods)
                    .ThenInclude(r => r.Food)
                    .FirstAsync(f => f.Id == Id);
            }
            Model ??= new();
        }

        public async Task Submit()
        {
            if (Model == null)
                return;

            var user = await authenticationService.GetUserAsync();

            var userId = user?.Id;

            if (userId == null)
                return;

            Model.UserId = userId;

            Model.SetNutrients();

            dbContext.Recipes.Update(Model);
            await dbContext.SaveChangesAsync();
            await jsRuntime.InvokeVoidAsync("history.back");
        }

        public async Task<IEnumerable<Food>> Search(string value, CancellationToken token)
        {
            var foods = dbContext.Foods.OrderBy(f => f.BrandRestaurant).AsQueryable();

            if (!string.IsNullOrWhiteSpace(value))
            {
                foods = foods.Where(f => f.BrandRestaurant.Contains(value) || f.Description.Contains(value));
            }

            var foodsList = await foods.Take(20).ToListAsync();

            return foodsList
                .OrderBy(n => n.Name)
                .ToList();
        }

        public void IngredientUpdated()
        {
            Model?.SetNutrients();
            StateHasChanged();
        }
    }
}
