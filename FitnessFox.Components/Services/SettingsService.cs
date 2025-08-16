using FitnessFox.Components.Data.Settings;
using FitnessFox.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        public async Task<T?> GetValue<T>(string? key)
        {
            var user = await authenticationService.GetUserAsync();
            if (user == null)
                return default;

            var value = applicationDbContext.UserSettings.Find(key, user.Id)?.Value;
            try
            {
                return JsonConvert.DeserializeObject<T?>(value ?? "");
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task SetValue<T>(string key, T? value)
        {
            var user = await authenticationService.GetUserAsync();
            if (user == null)
                return;

            var setting = applicationDbContext.UserSettings.Find(key, user.Id);
            if (setting != null)
            {
                setting.Value = JsonConvert.SerializeObject(value);
            }
            else
            {
                setting = new()
                {
                    Id = key,
                    UserId = user.Id,
                    Value = JsonConvert.SerializeObject(value),
                };
                applicationDbContext.UserSettings.Add(setting);
            }

            await applicationDbContext.SaveChangesAsync();
        }

        public async Task<Dictionary<string, UserSetting>> GetKeys()
        {
            var user = await authenticationService.GetUserAsync();
            if (user == null)
                return [];

            var settings = await applicationDbContext.UserSettings.Where(u => u.UserId == user.Id).ToDictionaryAsync(u => u.Id, u => u);
            return settings;
        }

        public Task<T?> GetValue<T>(SettingKey key)
        {
            return GetValue<T?>(key.ToString());
        }

        public Task SetValue<T>(SettingKey key, T? value)
        {
            return SetValue<T>(key.ToString(), value);
        }
    }
}
