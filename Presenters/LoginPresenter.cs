using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using LibreLinkConnector.Models;
using LibreLinkConnector.Services;

namespace LibreLinkConnector.Presenters
{
    public class LoginPresenter : INotifyPropertyChanged
    {
        private readonly AppState _appState;
        private readonly LibreLinkApiClient _apiClient;
        
        private string _email = string.Empty;
        private bool _rememberMe;
        private bool _isLoginButtonEnabled = true;
        private string _statusText = string.Empty;
        private Brush _statusBrush = Brushes.Gray;
        private bool _isStatusVisible;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<LoginSuccessEventArgs>? LoginSuccess;

        #region Bindable Properties

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        public bool IsLoginButtonEnabled
        {
            get => _isLoginButtonEnabled;
            set => SetProperty(ref _isLoginButtonEnabled, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public Brush StatusBrush
        {
            get => _statusBrush;
            set => SetProperty(ref _statusBrush, value);
        }

        public bool IsStatusVisible
        {
            get => _isStatusVisible;
            set => SetProperty(ref _isStatusVisible, value);
        }

        #endregion

        public LoginPresenter(AppState appState, LibreLinkApiClient apiClient)
        {
            _appState = appState;
            _apiClient = apiClient;
        }

        public async Task InitializeAsync()
        {
            var storedCredentials = CredentialManager.LoadCredentials();

            if (storedCredentials != null && storedCredentials.RememberMe)
            {
                Email = storedCredentials.Email;
                RememberMe = true;

                // Try to login automatically
                ShowStatus("Logging in...", false);
                await PerformLoginAsync(storedCredentials.Password);
            }
        }

        public async Task PerformLoginAsync(string password)
        {
            var email = Email.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowStatus("Please enter email and password.", true);
                return;
            }

            IsLoginButtonEnabled = false;
            ShowStatus("Logging in...", false);

            try
            {
                await LoginAsync(email, password);

                // Save credentials if "Remember me" is checked
                if (RememberMe && _appState.Session.IsAuthenticated)
                {
                    CredentialManager.SaveCredentials(new CredentialManager.StoredCredentials
                    {
                        Email = email,
                        Password = password,
                        RememberMe = true
                    });
                }
            }
            finally
            {
                IsLoginButtonEnabled = true;
            }
        }

        private async Task LoginAsync(string email, string password)
        {
            try
            {
                var loginResponse = await _apiClient.LoginAsync(email, password);

                if (loginResponse?.Status == 0)
                {
                    // Get connections
                    var connectionsResponse = await _apiClient.GetConnectionsAsync();

                    if (connectionsResponse?.Data != null && connectionsResponse.Data.Count > 0)
                    {
                        var connection = connectionsResponse.Data[0];
                        
                        // Update session
                        _appState.Session.Email = email;
                        _appState.Session.PatientId = connection.PatientId;
                        _appState.Session.PatientFirstName = connection.FirstName;
                        _appState.Session.PatientLastName = connection.LastName;
                        _appState.Session.TargetHigh = connection.TargetHigh;
                        _appState.Session.TargetLow = connection.TargetLow;

                        // Notify successful login
                        LoginSuccess?.Invoke(this, new LoginSuccessEventArgs { Session = _appState.Session });
                    }
                    else
                    {
                        ShowStatus("No patient connections found.", true);
                    }
                }
                else
                {
                    ShowStatus($"Login failed. Server returned status: {loginResponse?.Status}. Please check your credentials.", true);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nDetails: {ex.InnerException.Message}";
                }
                ShowStatus(errorMessage, true);
            }
        }

        private void ShowStatus(string message, bool isError)
        {
            StatusText = message;
            StatusBrush = isError
                ? new SolidColorBrush(Colors.Red)
                : new SolidColorBrush(Colors.Gray);
            IsStatusVisible = true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class LoginSuccessEventArgs : EventArgs
    {
        public UserSession? Session { get; set; }
    }
}
