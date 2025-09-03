
namespace FitnessFox.Components.Services
{
    public interface IThemeService
    {
        event Action OnChange;
        bool IsDarkMode { get; set; }
    }
}