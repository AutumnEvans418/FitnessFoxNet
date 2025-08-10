using FitnessFox.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.Services
{

    public class SettingsService : ISettingsService
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IAuthenticationService authenticationService;

        public SettingsService(
            ApplicationDbContext applicationDbContext, 
            IAuthenticationService authenticationService)
        {
            this.applicationDbContext = applicationDbContext;
            this.authenticationService = authenticationService;
        }
        public async Task<string?> GetValue(string key)
        {
            var user = await authenticationService.GetUserAsync();
            if (user == null)
                return null;

            return applicationDbContext.UserSettings.Find(key, user.Id)?.Value;
        }

        public async Task SetValue(string key, string value)
        {
            var user = await authenticationService.GetUserAsync();
            if (user == null)
                return;

            var setting = applicationDbContext.UserSettings.Find(key, user.Id);
            if (setting != null)
            {
                setting.Value = value;
            }
            else
            {
                setting = new()
                {
                    Id = key,
                    UserId = user.Id,
                    Value = value,
                };
                applicationDbContext.UserSettings.Add(setting);
            }

            await applicationDbContext.SaveChangesAsync();
        }

        public async Task<Dictionary<string, string?>> GetKeys()
        {
            var user = await authenticationService.GetUserAsync();
            if (user == null)
                return [];

            var settings = await applicationDbContext.UserSettings.Where(u => u.UserId == user.Id).ToDictionaryAsync(u => u.Id, u => u.Value);
            return settings;
        }
    }
}
