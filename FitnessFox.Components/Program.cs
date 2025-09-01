using FitnessFox.Components.Services;
using FitnessFox.Components.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components
{
    public static class Program
    {
        public static IServiceCollection RegisterComponentDependencies(this IServiceCollection services) 
        {
            
            services.AddScoped<ISyncService, GoogleSyncService>();
            services.AddScoped<IGoogleSheetsServices, GoogleSheetsServices>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<IFakeDataGenerator, FakeDataGenerator>();

            services.AddScoped<ILoadingService, LoadingService>();

            var viewModels = typeof(ViewModelBase).Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(ViewModelBase)))
                .ToList();

            foreach (var viewModel in viewModels)
            {
                services.AddTransient(viewModel);
            }

            return services;
        }
    }
}
