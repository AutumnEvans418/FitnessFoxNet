
using FitnessFox.Components.Data.Settings;

namespace FitnessFox.Components.Services
{
    public interface ISettingsService
    {
        Task<Dictionary<string, string?>> GetKeys();
        Task<string?> GetValue(string key);
        Task<string?> GetValue(SettingKey key);
        Task SetValue(string key, string? value);
        Task SetValue(SettingKey key, string? value);
    }
}