using FitnessFox.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessFox.Mobile
{
    public class ApplicationDbContextStateProvider : AuthenticationStateProvider
    {
        private readonly ApplicationDbContext dbContext;

        public ApplicationDbContextStateProvider(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await dbContext.Users.FirstOrDefaultAsync();

            if (user == null)
            {
                user = new ApplicationUser() { Id = "26218fa1-7163-4b42-aa57-51ef66515203" };
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }

            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            ], "Custom Authentication");

            var userC = new ClaimsPrincipal(identity);

            return new AuthenticationState(userC);
        }
    }
}