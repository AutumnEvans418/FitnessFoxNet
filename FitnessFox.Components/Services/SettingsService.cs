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

    public class SettingsService
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IAuthenticationService authenticationService;

        public SettingsService(ApplicationDbContext applicationDbContext, IAuthenticationService authenticationService)
        {
            this.applicationDbContext = applicationDbContext;
            this.authenticationService = authenticationService;
        }
        public async Task<string?> GetValue(string key)
        {
            var user = await authenticationService.GetUserAsync();

            return applicationDbContext.UserSettings.First(x => x.Key == key && user.Id == x.UserId).Value;
        }

        public async Task SetValue(string key, string value)
        {
            var user = await authenticationService.GetUserAsync();


        }
    }
}
