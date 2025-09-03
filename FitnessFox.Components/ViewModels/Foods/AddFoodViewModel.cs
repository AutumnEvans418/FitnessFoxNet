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
    public class AddFoodViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAuthenticationService authenticationService;
        private readonly IJSRuntime jsRuntime;

        public AddFoodViewModel(
            IDialogService mudDialogInstance, 
            ILoggingService loggingService,
            ApplicationDbContext applicationDbContext,
            IAuthenticationService authenticationService,
            IJSRuntime jsRuntime,
            ILoadingService loadingService) 
            : base(mudDialogInstance, loggingService, loadingService)
        {
            this.dbContext = applicationDbContext;
            this.authenticationService = authenticationService;
            this.jsRuntime = jsRuntime;
        }

        public Food? Model { get; set; }
        public List<string> Units { get; set; } = [];

        public string? Id { get; set; }

        public override async Task OnInitializedAsync()
        {
            await Load(Refresh);
        }

        public async Task Refresh()
        {
            if (Id != null)
            {
                var guid = Guid.Parse(Id);
                Model = await dbContext.Foods.FirstAsync(f => f.Id == guid);
            }
            Model ??= new();

            Units = await dbContext.Foods.Select(f => f.ServingUnit).Distinct().ToListAsync();
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

            dbContext.Foods.Update(Model);
            await dbContext.SaveChangesAsync();
            await jsRuntime.InvokeVoidAsync("history.back");
        }

        public Task<IEnumerable<string>> Search(string value, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Task.FromResult<IEnumerable<string>>(Units);

            return Task.FromResult<IEnumerable<string>>(Units.Where(u => u.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList());
        }
    }
}
