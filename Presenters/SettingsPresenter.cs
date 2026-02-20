using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using LibreLinkConnector.Services;

namespace LibreLinkConnector.Presenters
{
    public class SettingsPresenter : INotifyPropertyChanged
    {
        private int _updateIntervalMinutes;
        private bool _useEuropeanServer;
        private bool _showNotifications;
        private bool _isMgDlSelected;
        private int _highThresholdMgDl;
        private int _lowThresholdMgDl;
        private double _highThresholdMmol;
        private double _lowThresholdMmol;
        private Visibility _mgDlPanelVisibility;
        private Visibility _mmolLPanelVisibility;
        private bool _isLightTheme;
        private bool _isDarkTheme;
        private bool _isSystemTheme;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? SettingsChanged;

        public SettingsPresenter()
        {
            LoadSettings();
        }

        // Properties
        public int UpdateIntervalMinutes
        {
            get => _updateIntervalMinutes;
            set
            {
                if (_updateIntervalMinutes != value)
                {
                    _updateIntervalMinutes = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool UseEuropeanServer
        {
            get => _useEuropeanServer;
            set
            {
                if (_useEuropeanServer != value)
                {
                    _useEuropeanServer = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseGlobalServer));
                }
            }
        }

        public bool UseGlobalServer
        {
            get => !_useEuropeanServer;
            set
            {
                if (_useEuropeanServer != !value)
                {
                    _useEuropeanServer = !value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UseEuropeanServer));
                }
            }
        }

        public bool ShowNotifications
        {
            get => _showNotifications;
            set
            {
                if (_showNotifications != value)
                {
                    _showNotifications = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsMgDlSelected
        {
            get => _isMgDlSelected;
            set
            {
                if (_isMgDlSelected != value)
                {
                    _isMgDlSelected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMmolLSelected));
                    UpdateThresholdPanelVisibility();
                }
            }
        }

        public bool IsMmolLSelected
        {
            get => !_isMgDlSelected;
            set
            {
                if (_isMgDlSelected != !value)
                {
                    _isMgDlSelected = !value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMgDlSelected));
                    UpdateThresholdPanelVisibility();
                }
            }
        }

        public int HighThresholdMgDl
        {
            get => _highThresholdMgDl;
            set
            {
                if (_highThresholdMgDl != value)
                {
                    _highThresholdMgDl = value;
                    OnPropertyChanged();
                }
            }
        }

        public int LowThresholdMgDl
        {
            get => _lowThresholdMgDl;
            set
            {
                if (_lowThresholdMgDl != value)
                {
                    _lowThresholdMgDl = value;
                    OnPropertyChanged();
                }
            }
        }

        public double HighThresholdMmol
        {
            get => _highThresholdMmol;
            set
            {
                if (Math.Abs(_highThresholdMmol - value) > 0.001)
                {
                    _highThresholdMmol = value;
                    OnPropertyChanged();
                }
            }
        }

        public double LowThresholdMmol
        {
            get => _lowThresholdMmol;
            set
            {
                if (Math.Abs(_lowThresholdMmol - value) > 0.001)
                {
                    _lowThresholdMmol = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility MgDlPanelVisibility
        {
            get => _mgDlPanelVisibility;
            private set
            {
                if (_mgDlPanelVisibility != value)
                {
                    _mgDlPanelVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility MmolLPanelVisibility
        {
            get => _mmolLPanelVisibility;
            private set
            {
                if (_mmolLPanelVisibility != value)
                {
                    _mmolLPanelVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UpdateIntervalDisplay => $"{UpdateIntervalMinutes} min";

        public bool IsLightTheme
        {
            get => _isLightTheme;
            set
            {
                if (_isLightTheme != value)
                {
                    _isLightTheme = value;
                    OnPropertyChanged();
                    if (value)
                    {
                        IsDarkTheme = false;
                        IsSystemTheme = false;
                    }
                }
            }
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (_isDarkTheme != value)
                {
                    _isDarkTheme = value;
                    OnPropertyChanged();
                    if (value)
                    {
                        IsLightTheme = false;
                        IsSystemTheme = false;
                    }
                }
            }
        }

        public bool IsSystemTheme
        {
            get => _isSystemTheme;
            set
            {
                if (_isSystemTheme != value)
                {
                    _isSystemTheme = value;
                    OnPropertyChanged();
                    if (value)
                    {
                        IsLightTheme = false;
                        IsDarkTheme = false;
                    }
                }
            }
        }

        // Methods
        public void LoadSettings()
        {
            AppSettings.Load();

            UpdateIntervalMinutes = AppSettings.UpdateIntervalMinutes;
            UseEuropeanServer = AppSettings.UseEuropeanServer;
            ShowNotifications = AppSettings.ShowNotifications;
            IsMgDlSelected = AppSettings.PreferredUnit == GlucoseUnit.MgPerDl;
            HighThresholdMgDl = AppSettings.HighGlucoseThreshold;
            LowThresholdMgDl = AppSettings.LowGlucoseThreshold;
            HighThresholdMmol = AppSettings.HighGlucoseThresholdMmol;
            LowThresholdMmol = AppSettings.LowGlucoseThresholdMmol;

            // Load theme
            IsLightTheme = AppSettings.Theme == AppTheme.Light;
            IsDarkTheme = AppSettings.Theme == AppTheme.Dark;
            IsSystemTheme = AppSettings.Theme == AppTheme.System;

            UpdateThresholdPanelVisibility();
        }

        private void UpdateThresholdPanelVisibility()
        {
            MgDlPanelVisibility = IsMgDlSelected ? Visibility.Visible : Visibility.Collapsed;
            MmolLPanelVisibility = IsMmolLSelected ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ValidateAndSave()
        {
            // Validate mg/dL thresholds
            if (HighThresholdMgDl <= 0)
            {
                MessageBox.Show("Please enter a valid high glucose threshold (mg/dL).", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (LowThresholdMgDl <= 0)
            {
                MessageBox.Show("Please enter a valid low glucose threshold (mg/dL).", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (HighThresholdMgDl <= LowThresholdMgDl)
            {
                MessageBox.Show("High threshold must be greater than low threshold (mg/dL).", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate mmol/L thresholds
            if (HighThresholdMmol <= 0)
            {
                MessageBox.Show("Please enter a valid high glucose threshold (mmol/L).", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (LowThresholdMmol <= 0)
            {
                MessageBox.Show("Please enter a valid low glucose threshold (mmol/L).", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (HighThresholdMmol <= LowThresholdMmol)
            {
                MessageBox.Show("High threshold must be greater than low threshold (mmol/L).", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                // Save settings
                AppSettings.UpdateIntervalMinutes = UpdateIntervalMinutes;
                AppSettings.UseEuropeanServer = UseEuropeanServer;
                AppSettings.ShowNotifications = ShowNotifications;
                AppSettings.PreferredUnit = IsMgDlSelected ? GlucoseUnit.MgPerDl : GlucoseUnit.MmolPerL;
                AppSettings.HighGlucoseThreshold = HighThresholdMgDl;
                AppSettings.LowGlucoseThreshold = LowThresholdMgDl;
                AppSettings.HighGlucoseThresholdMmol = HighThresholdMmol;
                AppSettings.LowGlucoseThresholdMmol = LowThresholdMmol;

                // Save theme
                AppTheme selectedTheme = IsLightTheme ? AppTheme.Light : IsDarkTheme ? AppTheme.Dark : AppTheme.System;
                AppSettings.Theme = selectedTheme;

                AppSettings.Save();

                // Apply theme immediately
                ThemeManager.ApplyTheme(selectedTheme);

                // Notify that settings changed
                SettingsChanged?.Invoke(this, EventArgs.Empty);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
