using FitnessFox.Components.Services;
using FitnessFox.Data;
using FitnessFox.Data.Goals;
using FitnessFox.Data.Vitals;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace FitnessFox.Components.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly ISettingsService settingsService;
        private readonly ISyncService googleSyncService;
        private readonly ISnackbar snackbar;
        private readonly ApplicationDbContext dbContext;

        public HomeViewModel(
            IDialogService mudDialogInstance, 
            ILoggingService loggingService, 
            ILoadingService loadingService,
            IAuthenticationService authenticationService,
            ISettingsService settingsService,
            ISyncService googleSyncService,
            ISnackbar snackbar,
            ApplicationDbContext applicationDbContext) 
            : base(mudDialogInstance, loggingService, loadingService)
        {
            this.authenticationService = authenticationService;
            this.settingsService = settingsService;
            this.googleSyncService = googleSyncService;
            this.snackbar = snackbar;
            this.dbContext = applicationDbContext;
        }

        public DateTime? CurrentDate { get; set; } = DateTime.Now;

        public List<UserVital> Vitals { get; set; } = new();

        public UserVital Weight { get; set; } = new();
        public UserVital Systolic { get; set; } = new();
        public UserVital Diastolic { get; set; } = new();
        public UserVital Bpm { get; set; } = new();
        //public UserVital Temperature { get; set; } = new();
        //public UserVital Water { get; set; } = new();

        public ApplicationUser User { get; set; } = new();

        public UserGoal? WeightGoal { get; set; }

        public List<UserVital> BasicVitalsDisplay { get; set; } = new();

        public override async Task OnInitializedAsync()
        {
            await Load(async () =>
            {
                await SyncDb();
                await Refresh();
            });
            
        }

        private async Task SyncDb()
        {
            var shouldSync = await settingsService.GetValue<bool?>(Data.Settings.SettingKey.SyncOnStart);
            var lastDate = await settingsService.GetValue<DateTime?>(Data.Settings.SettingKey.SyncLastDate);

            if (shouldSync == true && (lastDate == null || lastDate < DateTime.Now.AddHours(-1)))
            {
                await googleSyncService.Sync();

                await settingsService.SetValue<DateTime?>(Data.Settings.SettingKey.SyncLastDate, DateTime.Now);

                snackbar.Add("Synced!");
            }
        }

        public async Task Refresh()
        {
            var user = await authenticationService.GetUserAsync();

            if (user == null)
                return;

            User = user;

            var currentDate = CurrentDate.GetValueOrDefault().Date;

            Vitals = await dbContext.UserVitals
                .Where(u => u.User.Id == user.Id)
                .Where(u => u.Date.Date == currentDate)
                .ToListAsync();

            var goals = await dbContext.UserGoals.Where(u => u.UserId == user.Id).ToListAsync();

            WeightGoal = goals.FirstOrDefault(g => g.Type == UserGoalType.WeightLbs);

            UserVital getType(UserVitalType type)
            {
                return Vitals.FirstOrDefault(v => v.Type == type) ?? new()
                {
                    UserId = user.Id,
                    Type = type,
                    Date = currentDate,
                };
            }

            Weight = getType(UserVitalType.Weight);
            Systolic = getType(UserVitalType.Systolic);
            Diastolic = getType(UserVitalType.Diastolic);
            Bpm = getType(UserVitalType.Bpm);

            BasicVitalsDisplay.Add(getType(UserVitalType.Temperature));
            BasicVitalsDisplay.Add(getType(UserVitalType.Water));
            BasicVitalsDisplay.Add(getType(UserVitalType.WaistIn));
            BasicVitalsDisplay.Add(getType(UserVitalType.UnderbustIn));
            BasicVitalsDisplay.Add(getType(UserVitalType.StandingBustIn));
            BasicVitalsDisplay.Add(getType(UserVitalType.LeaningBustIn));
        }

        public async Task SetDate(DateTime? date)
        {
            CurrentDate = date;
            await Refresh();
        }

        public async Task Save(params UserVital[] vitals)
        {
            foreach (var item in vitals)
            {
                dbContext.Update(item);
            }
            await dbContext.SaveChangesAsync();
        }

    }
}
