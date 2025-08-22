using FitnessFox.Components.Services;
using FitnessFox.Components.ViewModels;
using FitnessFox.Data;
using FitnessFox.Mobile.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace FitnessFox.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            builder.Services.AddScoped<AuthenticationStateProvider, ApplicationDbContextStateProvider>();

            builder.Services.AddScoped<ISyncService, GoogleSyncService>();
            builder.Services.AddScoped<ISettingsService, SettingsService>();
            builder.Services.AddScoped<IFileService, MauiFileService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ILoggingService, LoggingService>();

            builder.Services.AddScoped<ILoadingService, LoadingService>();

            var viewModels = typeof(ViewModelBase).Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(ViewModelBase)))
                .ToList();

            foreach (var viewModel in viewModels)
            {
                builder.Services.AddTransient(viewModel);
            }

            builder.Services.AddAuthorizationCore();

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite($"Data Source=" + Path.Combine(FileSystem.AppDataDirectory, "database.dat"));
            });
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();

#endif

            var app = builder.Build();

            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            return app;
        }
    }
}
