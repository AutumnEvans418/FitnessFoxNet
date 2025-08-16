using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System.Security.Claims;

namespace FitnessFox.Components.ViewModels
{
    public class RecipesViewModel : ViewModelBase
    {
        public RecipesViewModel(
            IDialogService mudDialogInstance, 
            ILoggingService loggingService, 
            ILoadingService loadingService,
            ApplicationDbContext dbContext) 
            : base(mudDialogInstance, loggingService, loadingService)
        {
            this.dbContext = dbContext;
        }

        string? searchString = null;
        private readonly ApplicationDbContext dbContext;
        public Func<Task>? ReloadServerData { get; set; }

        public async Task<GridData<Recipe>> ServerReload(GridState<Recipe> state)
        {
            var dataQuery = dbContext.Recipes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                dataQuery = dataQuery.Where(p => p.Name.Contains(searchString));
            }

            var totalItems = await dataQuery.CountAsync();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(Recipe.Name):
                        dataQuery = dataQuery.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Name
                        );
                        break;
                    case nameof(Recipe.NumberOfPeople):
                        dataQuery = dataQuery.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.NumberOfPeople
                        );
                        break;
                }
            }

            var pagedData = await dataQuery.Skip(state.Page * state.PageSize).Take(state.PageSize).ToListAsync();
            return new GridData<Recipe>
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }

        public async Task OnSearch(string text)
        {
            if (ReloadServerData == null)
                return;
            searchString = text;
            await ReloadServerData();
        }
    }
}
