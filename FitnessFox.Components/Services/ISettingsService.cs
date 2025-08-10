
namespace FitnessFox.Components.Services
{
    public interface ISettingsService
    {
        Task<Dictionary<string, string?>> GetKeys();
        Task<string?> GetValue(string key);
        Task SetValue(string key, string value);
    }
}