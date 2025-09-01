using FitnessFox.Components.Data.Settings;
using FitnessFox.Components.Services;
using FitnessFox.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FitnessFox.Components.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService settingsService;
        private readonly ISyncService googleSyncService;
        private readonly ISnackbar snackbar;
        private readonly ApplicationDbContext dbContext;
        private readonly NavigationManager nav;
        private readonly IFakeDataGenerator fakeDataGenerator;

        public SettingsViewModel(
            IDialogService mudDialogInstance,
            ILoggingService loggingService,
            ILoadingService loadingService,
            ISettingsService settingsService,
            ISyncService googleSyncService,
            ISnackbar snackbar,
            ApplicationDbContext dbContext,
            NavigationManager nav,
            IFakeDataGenerator fakeDataGenerator)
            : base(mudDialogInstance, loggingService, loadingService)
        {
            this.settingsService = settingsService;
            this.googleSyncService = googleSyncService;
            this.snackbar = snackbar;
            this.dbContext = dbContext;
            this.nav = nav;
            this.fakeDataGenerator = fakeDataGenerator;
        }

        public List<SettingKey> Keys { get; set; } = Enum.GetValues<SettingKey>().ToList();
        public Dictionary<string, UserSetting> KeyValues { get; set; } = new();

        public override async Task OnInitializedAsync()
        {
            await Load(Refresh);
        }

        public async Task Refresh()
        {
            KeyValues = await settingsService.GetKeys();
        }

        public async Task SetValue<T>(SettingKey key, T value)
        {
            await settingsService.SetValue(key, value);
            await Refresh();
        }

        public async Task Sync()
        {
            await googleSyncService.Sync();
            snackbar.Add("Synced!");
            await Refresh();
        }
        public async Task ClearDb()
        {
            var result = await dialogService.ShowMessageBox("Confirm", "Are you sure you want to delete ALL local data?", "Yes", "No");
            if (result == true)
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                nav.NavigateTo(nav.Uri, true);
            }
        }

        public async Task SeedDb()
        {
            var result = await dialogService.ShowMessageBox("Confirm", "Are you sure you want to seed the db with fake data?", "Yes", "No");
            if (result == true)
            {
                await fakeDataGenerator.SeedData();
                nav.NavigateTo(nav.Uri, true);
            }
        }
    }
}
