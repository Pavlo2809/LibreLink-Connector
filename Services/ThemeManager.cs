using System.Windows;
using Microsoft.Win32;

namespace LibreLinkConnector.Services
{
    public static class ThemeManager
    {
        private const string LightThemeUri = "Themes/LightTheme.xaml";
        private const string DarkThemeUri = "Themes/DarkTheme.xaml";

        public static void ApplyTheme(AppTheme theme)
        {
            var themeUri = GetThemeUri(theme);
            
            // Remove existing theme dictionaries
            var existingThemes = Application.Current.Resources.MergedDictionaries
                .Where(d => d.Source != null && 
                       (d.Source.OriginalString.Contains("LightTheme") || 
                        d.Source.OriginalString.Contains("DarkTheme")))
                .ToList();

            foreach (var existingTheme in existingThemes)
            {
                Application.Current.Resources.MergedDictionaries.Remove(existingTheme);
            }

            // Add new theme dictionary
            var newTheme = new ResourceDictionary
            {
                Source = new Uri(themeUri, UriKind.Relative)
            };
            
            Application.Current.Resources.MergedDictionaries.Insert(0, newTheme);
        }

        private static string GetThemeUri(AppTheme theme)
        {
            return theme switch
            {
                AppTheme.Light => LightThemeUri,
                AppTheme.Dark => DarkThemeUri,
                AppTheme.System => GetSystemTheme() == AppTheme.Dark ? DarkThemeUri : LightThemeUri,
                _ => LightThemeUri
            };
        }

        public static AppTheme GetSystemTheme()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = key?.GetValue("AppsUseLightTheme");
                
                if (value is int intValue)
                {
                    return intValue == 0 ? AppTheme.Dark : AppTheme.Light;
                }
            }
            catch
            {
                // If we can't read the registry, default to light theme
            }

            return AppTheme.Light;
        }

        public static AppTheme GetEffectiveTheme(AppTheme selectedTheme)
        {
            return selectedTheme == AppTheme.System ? GetSystemTheme() : selectedTheme;
        }
    }
}
