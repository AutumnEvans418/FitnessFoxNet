
using FitnessFox.Components.Data.Settings;

namespace FitnessFox.Components.Services
{
    public interface ISettingsService
    {
        Task<Dictionary<string, UserSetting>> GetKeys();
        Task<T?> GetValue<T>(string key);
        Task<T?> GetValue<T>(SettingKey key);
        Task SetValue<T>(string key, T? value);
        Task SetValue<T>(SettingKey key, T? value);
    }
}