using System.IO;
using Newtonsoft.Json;

namespace LibreLinkConnector
{
    public enum GlucoseUnit
    {
        MgPerDl = 0,
        MmolPerL = 1
    }

    public enum AppTheme
    {
        Light = 0,
        Dark = 1,
        System = 2
    }

    public class AppSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LibreLinkConnector",
            "settings.json"
        );

        public static int UpdateIntervalMinutes { get; set; } = 5;
        public static bool UseEuropeanServer { get; set; } = true;
        public static bool ShowNotifications { get; set; } = true;
        public static GlucoseUnit PreferredUnit { get; set; } = GlucoseUnit.MgPerDl;
        public static int HighGlucoseThreshold { get; set; } = 180;
        public static int LowGlucoseThreshold { get; set; } = 70;
        public static double HighGlucoseThresholdMmol { get; set; } = 10.0;
        public static double LowGlucoseThresholdMmol { get; set; } = 3.9;
        public static AppTheme Theme { get; set; } = AppTheme.System;
        public static bool IsWidgetMode { get; set; } = false;

        public static void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var settings = new
                {
                    UpdateIntervalMinutes,
                    UseEuropeanServer,
                    ShowNotifications,
                    PreferredUnit = (int)PreferredUnit,
                    HighGlucoseThreshold,
                    LowGlucoseThreshold,
                    HighGlucoseThresholdMmol,
                    LowGlucoseThresholdMmol,
                    Theme = (int)Theme,
                    IsWidgetMode
                };

                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save settings: {ex.Message}", ex);
            }
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    return;
                }

                var json = File.ReadAllText(SettingsPath);
                var settings = JsonConvert.DeserializeObject<dynamic>(json);

                if (settings != null)
                {
                    UpdateIntervalMinutes = settings.UpdateIntervalMinutes ?? 5;
                    UseEuropeanServer = settings.UseEuropeanServer ?? true;
                    ShowNotifications = settings.ShowNotifications ?? true;
                    PreferredUnit = (GlucoseUnit)(settings.PreferredUnit ?? 0);
                    HighGlucoseThreshold = settings.HighGlucoseThreshold ?? 180;
                    LowGlucoseThreshold = settings.LowGlucoseThreshold ?? 70;
                    Theme = (AppTheme)(settings.Theme ?? 2);
                    IsWidgetMode = settings.IsWidgetMode ?? false;
                    HighGlucoseThresholdMmol = settings.HighGlucoseThresholdMmol ?? 10.0;
                    LowGlucoseThresholdMmol = settings.LowGlucoseThresholdMmol ?? 3.9;
                }
            }
            catch (Exception)
            {
                // If loading fails, use defaults
            }
        }
    }
}
