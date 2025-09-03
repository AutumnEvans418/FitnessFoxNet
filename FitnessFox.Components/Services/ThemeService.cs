namespace FitnessFox.Components.Services
{
    public class ThemeService : IThemeService
    {
        public event Action? OnChange;
        
        private bool _isDarkMode;
        public bool IsDarkMode 
        { 
            get => _isDarkMode; 
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    NotifyStateChanged();
                }
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}