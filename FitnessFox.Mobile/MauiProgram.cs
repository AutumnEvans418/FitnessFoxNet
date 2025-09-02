using FitnessFox.Components;
using FitnessFox.Components.Data.Options;
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
            builder.Services.AddScoped<IFileService, MauiFileService>();

            builder.Services.RegisterComponentDependencies();

            builder.Services.AddAuthorizationCore();

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite($"Data Source=" + Path.Combine(FileSystem.AppDataDirectory, "database.dat"));
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.Configure<DatabaseOptions>(o =>
            {
                o.ShowSeedDatabase = true;
                o.ShowClearDatabase = true;
            });

            var app = builder.Build();

            using var scope = app.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();



            return app;
        }
    }
}
