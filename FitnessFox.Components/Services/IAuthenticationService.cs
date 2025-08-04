using FitnessFox.Data;

namespace FitnessFox.Components.Services
{
    public interface IAuthenticationService
    {
        Task<ApplicationUser?> GetUserAsync();
    }
}