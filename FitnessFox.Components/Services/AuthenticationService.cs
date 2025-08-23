using FitnessFox.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessFox.Components.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ApplicationDbContext applicationDbContext;

        public AuthenticationService(
            AuthenticationStateProvider authenticationStateProvider,
            ApplicationDbContext applicationDbContext)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<ApplicationUser?> GetUserAsync()
        {
            var user = await authenticationStateProvider.GetAuthenticationStateAsync();

            var userId = user.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return null;
            return await applicationDbContext.Users.AsNoTracking().FirstOrDefaultAsync(p => p.Id == userId);
        }
    }
}
