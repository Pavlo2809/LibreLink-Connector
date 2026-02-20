using System.Windows;
using LibreLinkConnector.Services;

namespace LibreLinkConnector
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Load settings
            AppSettings.Load();
            
            // Apply theme
            ThemeManager.ApplyTheme(AppSettings.Theme);
        }
    }
}
