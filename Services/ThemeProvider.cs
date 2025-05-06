using MudBlazor;

namespace QuadMasterApp.Services
{
    // Interface for the theme provider
    public interface IThemeProvider
    {
        bool IsDarkMode { get; }
        MudTheme Theme { get; }
        event Action ThemeChanged;
        void SetDarkMode(bool isDarkMode);
    }

    // Implementation of the theme provider
    public class ThemeProvider : IThemeProvider
    {
        private bool _isDarkMode;
        private readonly MudTheme _theme;

        public bool IsDarkMode => _isDarkMode;
        public MudTheme Theme => _theme;

        public event Action ThemeChanged;

        public ThemeProvider(MudTheme theme, bool initialDarkMode = false)
        {
            _theme = theme;
            _isDarkMode = initialDarkMode;
        }

        public void SetDarkMode(bool isDarkMode)
        {
            _isDarkMode = isDarkMode;
            ThemeChanged?.Invoke();
        }
    }
}