using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibreLinkConnector.Models;
using LibreLinkConnector.Presenters;
using LibreLinkConnector.Services;
using LibreLinkConnector.Views;

namespace LibreLinkConnector
{
    public partial class MainWindow : Window
    {
        private readonly AppState _appState;
        private readonly LibreLinkApiClient _apiClient;
        
        private readonly LoginPresenter _loginPresenter;
        private readonly ForecastPresenter _forecastPresenter;
        private readonly SettingsPresenter _settingsPresenter;
        
        private readonly LoginView _loginView;
        private readonly ForecastView _forecastView;
        private readonly SettingsView _settingsView;
        private readonly WidgetView _widgetView;
        
        private bool _isWidgetMode = false;

        public MainWindow()
        {
            InitializeComponent();

            // Load settings
            AppSettings.Load();

            // Create shared instances
            _appState = new AppState();
            _apiClient = new LibreLinkApiClient();

            // Create presenters
            _loginPresenter = new LoginPresenter(_appState, _apiClient);
            _forecastPresenter = new ForecastPresenter(_appState, _apiClient);
            _settingsPresenter = new SettingsPresenter();

            // Create views
            _loginView = new LoginView(_loginPresenter);
            _forecastView = new ForecastView(_forecastPresenter);
            _settingsView = new SettingsView(_settingsPresenter);
            _widgetView = new WidgetView(_forecastPresenter);

            // Wire up events
            _loginPresenter.LoginSuccess += OnLoginSuccess;
            _settingsPresenter.SettingsChanged += OnSettingsChanged;
            _settingsView.SaveRequested += OnSettingsSaveRequested;
            _settingsView.CancelRequested += OnSettingsCancelRequested;
            _widgetView.ExpandRequested += OnWidgetExpandRequested;

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply theme
            ThemeManager.ApplyTheme(AppSettings.Theme);

            // Show login view initially
            ShowView(_loginView);
            UpdateHeaderVisibility(ViewMode.Login);

            // Try auto-login
            await _loginPresenter.InitializeAsync();
        }

        private void OnLoginSuccess(object? sender, LoginSuccessEventArgs e)
        {
            // Switch to forecast view
            Dispatcher.Invoke(async () =>
            {
                ShowView(_forecastView);
                UpdateHeaderVisibility(ViewMode.Forecast);
                _appState.CurrentView = ViewMode.Forecast;

                await _forecastPresenter.InitializeAsync();
            });
        }

        private void ShowView(UserControl view)
        {
            MainContentControl.Content = view;
        }

        private void UpdateHeaderVisibility(ViewMode viewMode)
        {
            LogoutButton.Visibility = viewMode == ViewMode.Forecast ? Visibility.Visible : Visibility.Collapsed;
            SettingsButton.Visibility = viewMode == ViewMode.Forecast ? Visibility.Visible : Visibility.Collapsed;
            WidgetButton.Visibility = viewMode == ViewMode.Forecast ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Ask if user wants to delete saved credentials
            var result = MessageBox.Show(
                "Do you want to delete saved credentials?",
                "Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                CredentialManager.DeleteCredentials();
            }

            // Stop updates
            _forecastPresenter.StopUpdates();

            // Clear session
            _appState.ClearSession();

            // Return to login view
            ShowView(_loginView);
            UpdateHeaderVisibility(ViewMode.Login);
            _appState.CurrentView = ViewMode.Login;

            // Reset login presenter
            _ = _loginPresenter.InitializeAsync();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Show settings view
            ShowView(_settingsView);
        }

        private void OnSettingsSaveRequested(object? sender, EventArgs e)
        {
            // Return to forecast view
            ShowView(_forecastView);
            
            MessageBox.Show(
                "Settings updated successfully. If you changed the server region, you may need to logout and login again.",
                "Settings Updated",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void OnSettingsCancelRequested(object? sender, EventArgs e)
        {
            // Return to forecast view
            ShowView(_forecastView);
        }

        private void OnSettingsChanged(object? sender, EventArgs e)
        {
            // Notify forecast presenter to refresh with new settings
            _forecastPresenter.OnSettingsChanged();
        }

        private void ToggleWidgetButton_Click(object sender, RoutedEventArgs e)
        {
            _isWidgetMode = !_isWidgetMode;
            _appState.IsWidgetMode = _isWidgetMode;
            
            AppSettings.IsWidgetMode = _isWidgetMode;
            AppSettings.Save();

            if (_isWidgetMode)
            {
                // Switch to widget mode
                NormalModeGrid.Visibility = Visibility.Collapsed;
                WidgetModeGrid.Visibility = Visibility.Visible;
                WidgetContentControl.Content = _widgetView;
                
                Width = 280;
                Height = 280;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Topmost = true;
            }
            else
            {
                // Switch to normal mode
                NormalModeGrid.Visibility = Visibility.Visible;
                WidgetModeGrid.Visibility = Visibility.Collapsed;
                
                Width = 950;
                Height = 750;
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
                Topmost = false;
            }
        }

        private void OnWidgetExpandRequested(object? sender, EventArgs e)
        {
            // Expand from widget mode to normal mode
            ToggleWidgetButton_Click(sender!, new RoutedEventArgs());
        }

        private void WidgetBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Double-click to expand
                ToggleWidgetButton_Click(sender, new RoutedEventArgs());
            }
            else
            {
                // Single click to drag
                DragMove();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _forecastPresenter.Dispose();
            base.OnClosing(e);
        }
    }
}
