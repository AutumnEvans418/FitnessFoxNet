using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Foods;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.ViewModels.Foods
{
    public class FoodIndexViewModel : ViewModelBase
    {
        public FoodIndexViewModel(
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

        public async Task<GridData<Food>> ServerReload(GridState<Food> state)
        {
            var dataQuery = dbContext.Foods.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                dataQuery = dataQuery.Where(p => p.BrandRestaurant.Contains(searchString) || p.Description.Contains(searchString));
            }

            var totalItems = await dataQuery.CountAsync();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(Food.BrandRestaurant):
                        dataQuery = dataQuery.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.BrandRestaurant
                        );
                        break;
                    case nameof(Food.Description):
                        dataQuery = dataQuery.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Description
                        );
                        break;
                }
            }

            var pagedData = await dataQuery.Skip(state.Page * state.PageSize).Take(state.PageSize).ToListAsync();
            return new GridData<Food>
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
