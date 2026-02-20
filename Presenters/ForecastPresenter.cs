using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using LibreLinkConnector.Models;
using LibreLinkConnector.Services;

namespace LibreLinkConnector.Presenters
{
    public class ForecastPresenter : INotifyPropertyChanged, IDisposable
    {
        private readonly AppState _appState;
        private readonly LibreLinkApiClient _apiClient;
        private readonly DispatcherTimer _autoUpdateTimer;

        // Bindable UI Properties
        private string _currentValueText = "--";
        private string _trendArrowText = "→";
        private string _unitText = "mg/dL";
        private string _statusText = "Normal";
        private Brush _statusBrush = Brushes.Green;
        private Brush _currentValueBrush = Brushes.Black;
        private string _lastUpdateText = "--";
        private string _patientNameText = "--";
        private string _targetHighText = "180 mg/dL";
        private string _targetLowText = "70 mg/dL";
        private string _autoUpdateStatusText = "Auto-update enabled";
        private List<GlucoseMeasurement> _historyItems = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Bindable Properties

        public string CurrentValueText
        {
            get => _currentValueText;
            set => SetProperty(ref _currentValueText, value);
        }

        public string TrendArrowText
        {
            get => _trendArrowText;
            set => SetProperty(ref _trendArrowText, value);
        }

        public string UnitText
        {
            get => _unitText;
            set => SetProperty(ref _unitText, value);
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

        public Brush CurrentValueBrush
        {
            get => _currentValueBrush;
            set => SetProperty(ref _currentValueBrush, value);
        }

        public string LastUpdateText
        {
            get => _lastUpdateText;
            set => SetProperty(ref _lastUpdateText, value);
        }

        public string PatientNameText
        {
            get => _patientNameText;
            set => SetProperty(ref _patientNameText, value);
        }

        public string TargetHighText
        {
            get => _targetHighText;
            set => SetProperty(ref _targetHighText, value);
        }

        public string TargetLowText
        {
            get =>_targetLowText;
            set => SetProperty(ref _targetLowText, value);
        }

        public string AutoUpdateStatusText
        {
            get => _autoUpdateStatusText;
            set => SetProperty(ref _autoUpdateStatusText, value);
        }

        public List<GlucoseMeasurement> HistoryItems
        {
            get => _historyItems;
            set => SetProperty(ref _historyItems, value);
        }

        #endregion

        public ForecastPresenter(AppState appState, LibreLinkApiClient apiClient)
        {
            _appState = appState;
            _apiClient = apiClient;

            // Setup auto-update timer
            _autoUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(AppSettings.UpdateIntervalMinutes)
            };
            _autoUpdateTimer.Tick += AutoUpdateTimer_Tick;
        }

        public async Task InitializeAsync()
        {
            // Update UI with patient info
            PatientNameText = _appState.Session.PatientFullName;
            
            // Load initial data
            await RefreshDataAsync();

            // Start auto-update timer
            _autoUpdateTimer.Start();
        }

        private async void AutoUpdateTimer_Tick(object? sender, EventArgs e)
        {
            // Errors should not stop the refresh cycle
            await RefreshDataAsync();
        }

        public async Task RefreshDataAsync()
        {
            if (string.IsNullOrEmpty(_appState.Session.PatientId))
                return;

            try
            {
                AutoUpdateStatusText = "Updating...";

                var graphResponse = await _apiClient.GetGraphDataAsync(_appState.Session.PatientId);

                if (graphResponse?.Data?.Connection != null)
                {
                    var connection = graphResponse.Data.Connection;
                    var measurement = connection.GlucoseMeasurement;

                    if (measurement != null)
                    {
                        // Update model
                        _appState.GlucoseData.CurrentValue = measurement.ValueInMgPerDl;
                        _appState.GlucoseData.TrendArrow = measurement.TrendArrow ?? 0;
                        _appState.GlucoseData.LastUpdateTime = DateTime.Now;
                        _appState.GlucoseData.Unit = AppSettings.PreferredUnit;
                        
                        // Update UI
                        UpdateCurrentReading();
                    }

                    // Update history
                    if (graphResponse.Data.GraphDataPoints != null)
                    {
                        var recentData = graphResponse.Data.GraphDataPoints
                            .OrderByDescending(g => g.Timestamp)
                            .Take(10)
                            .ToList();

                        _appState.GlucoseData.History = recentData;
                        HistoryItems = recentData;
                    }
                }

                var interval = AppSettings.UpdateIntervalMinutes;
                AutoUpdateStatusText = $"Last updated: {DateTime.Now:HH:mm:ss}. Next update in {interval} minute{(interval > 1 ? "s" : "")}.";
            }
            catch (Exception ex)
            {
                // Error should not stop the refresh cycle
                AutoUpdateStatusText = $"Update failed: {ex.Message}. Will retry in {AppSettings.UpdateIntervalMinutes} minute(s).";
            }
        }

        private void UpdateCurrentReading()
        {
            var glucoseData = _appState.GlucoseData;
            var session = _appState.Session;
            
            // Get thresholds based on unit
            double highThreshold, lowThreshold;
            
            if (glucoseData.Unit == GlucoseUnit.MgPerDl)
            {
                highThreshold = AppSettings.HighGlucoseThreshold;
                lowThreshold = AppSettings.LowGlucoseThreshold;
            }
            else
            {
                highThreshold = AppSettings.HighGlucoseThresholdMmol;
                lowThreshold = AppSettings.LowGlucoseThresholdMmol;
            }

            // Update display values
            CurrentValueText = glucoseData.DisplayValue.ToString("F1");
            TrendArrowText = glucoseData.TrendArrowSymbol;
            UnitText = glucoseData.UnitText;
            
            // Update status
            var status = glucoseData.GetStatus(highThreshold, lowThreshold);
            StatusText = glucoseData.GetStatusText(highThreshold, lowThreshold);
            
            StatusBrush = status switch
            {
                GlucoseStatus.High => new SolidColorBrush(Colors.OrangeRed),
                GlucoseStatus.Low => new SolidColorBrush(Colors.OrangeRed),
                GlucoseStatus.Normal => new SolidColorBrush(Colors.Green),
                _ => new SolidColorBrush(Colors.Gray)
            };
            
            CurrentValueBrush = status == GlucoseStatus.Normal 
                ? Application.Current.TryFindResource("TextBrush") as SolidColorBrush ?? new SolidColorBrush(Colors.White) 
                : new SolidColorBrush(Colors.OrangeRed);

            // Update timestamp
            LastUpdateText = glucoseData.LastUpdateTime.ToString("HH:mm:ss");

            // Update target ranges
            if (glucoseData.Unit == GlucoseUnit.MgPerDl)
            {
                TargetHighText = $"{session.TargetHigh} mg/dL";
                TargetLowText = $"{session.TargetLow} mg/dL";
            }
            else
            {
                double targetHighMmol = session.TargetHigh / 18.0;
                double targetLowMmol = session.TargetLow / 18.0;
                TargetHighText = $"{targetHighMmol:F1} mmol/L";
                TargetLowText = $"{targetLowMmol:F1} mmol/L";
            }

            // Update patient name
            PatientNameText = session.PatientFullName;
        }

        public void OnSettingsChanged()
        {
            // Update API client with new server URL
            _apiClient.UpdateBaseAddress();

            // Restart timer with new interval
            _autoUpdateTimer.Stop();
            _autoUpdateTimer.Interval = TimeSpan.FromMinutes(AppSettings.UpdateIntervalMinutes);
            _autoUpdateTimer.Start();

            // Update unit preference
            _appState.GlucoseData.Unit = AppSettings.PreferredUnit;
            
            // Refresh UI immediately
            UpdateCurrentReading();
        }

        public void StopUpdates()
        {
            _autoUpdateTimer.Stop();
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

        public void Dispose()
        {
            _autoUpdateTimer?.Stop();
        }
    }
}
