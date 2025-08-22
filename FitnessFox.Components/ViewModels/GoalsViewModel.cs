using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Goals;
using FitnessFox.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.ViewModels
{
    public class GoalsViewModel : ViewModelBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly ApplicationDbContext dbContext;

        public GoalsViewModel(
            IDialogService mudDialogInstance, 
            ILoggingService loggingService, 
            ILoadingService loadingService,
            IAuthenticationService authenticationService,
            ApplicationDbContext applicationDbContext) 
            : base(mudDialogInstance, loggingService, loadingService)
        {
            this.authenticationService = authenticationService;
            this.dbContext = applicationDbContext;
        }

        public List<UserGoalType> GoalTypes { get; set; } = Enum.GetValues<UserGoalType>().ToList();

        public List<UserGoal> UserGoals { get; set; } = new();
        public ApplicationUser User { get; set; } = new();

        public override async Task OnInitializedAsync()
        {
            await Load(Refresh);
        }

        public async Task Refresh()
        {
            UserGoals.Clear();

            var user = await authenticationService.GetUserAsync();
            if (user == null)
                return;

            User = user;

            var goals = await dbContext.UserGoals.Where(u => u.UserId == user.Id).ToListAsync();

            foreach (var type in GoalTypes)
            {
                var goal = goals.FirstOrDefault(u => u.Type == type) ?? new()
                {
                    Type = type,
                    UserId = user.Id,
                };
                UserGoals.Add(goal);
            }

            StateHasChanged();
        }
        
        public async Task Save(UserGoal goal)
        {
            if (goal.Type == UserGoalType.WeightLbs && User.HeightInches >= 0)
            {
                var bmi = UserGoals.First(b => b.Type == UserGoalType.Bmi);
                bmi.Value = FitnessExtensions.Bmi(goal.Value, User.HeightInches);
                dbContext.Update(bmi);
            }
            else if (goal.Type == UserGoalType.Bmi && User.HeightInches >= 0)
            {
                var weight = UserGoals.First(b => b.Type == UserGoalType.WeightLbs);
                weight.Value = FitnessExtensions.BmiWeight(goal.Value, User.HeightInches);
                dbContext.Update(weight);
            }

            dbContext.Update(goal);
            await dbContext.SaveChangesAsync();
            await Refresh();
        }
    }
}
