# LibreLink Connector

A Windows desktop application to monitor blood glucose levels using the LibreLinkUp API.

## Features

- ✅ **Widget Mode**: Compact always-on-top display for continuous monitoring without window switching
- ✅ **Modern UI**: Clean, easy-to-read interface displaying current glucose readings
- ✅ **Auto-Update**: Automatic refresh of glucose data at configurable intervals (default: 5 minutes)
- ✅ **Secure Credential Storage**: Encrypted storage of login credentials using Windows Data Protection API
- ✅ **Auto-Login**: Automatically log in on application start with saved credentials
- ✅ **Historical Data**: View recent glucose readings with trend indicators
- ✅ **Target Ranges**: Display target high/low thresholds
- ✅ **Trend Arrows**: Visual indicators showing glucose trends (rising, falling, stable)
- ✅ **Configurable Settings**: Customize update intervals, server region, and alert thresholds

## Download

### Latest Release: v1.0.0-beta

**[📥 Download v1.0.0-beta](https://github.com/Pavlo2809/LibreLink-Connector/releases/tag/v1.0.0-beta)**

#### What's Included:
- Pre-built Windows executable
- No installation required
- .NET 8.0 Runtime required (or use self-contained version)

For all releases, visit the [Releases page](https://github.com/Pavlo2809/LibreLink-Connector/releases).

## Screenshots

### Login Screen
The application starts with a secure login screen where you can enter your LibreLinkUp credentials.

### Dashboard
The main dashboard displays:
- Current blood glucose level
- Trend arrow (↑↑ rising rapidly, ↑ rising, → stable, ↓ falling, ↓↓ falling rapidly)
- Status indicator (Normal, High, Low)
- Target high/low ranges
- Last update timestamp
- Patient name
- Recent history of glucose readings

## Requirements

- Windows 10 or later
- .NET 8.0 Runtime
- LibreLinkUp account with access to patient data

## Installation

### Option 1: Download Pre-built Release (Recommended)

1. Download the latest release from the [Releases page](https://github.com/Pavlo2809/LibreLink-Connector/releases/latest)
2. Extract the ZIP file to a folder of your choice
3. Run `LibreLinkConnector.exe`

**Note**: Requires [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (or download the self-contained version which includes the runtime)

### Option 2: Build from Source

1. Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Clone or download this repository
3. Open a terminal in the project directory
4. Run the following commands:

```powershell
dotnet restore
dotnet build
dotnet run
```

### Option 3: Build Release Version

```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

The executable will be in `bin\Release\net8.0-windows\win-x64\publish\`

## Usage

### First Time Setup

1. Launch the application
2. Enter your LibreLinkUp email and password
3. Check "Remember me" if you want to save your credentials for auto-login
4. Click "Login"

### Main Features

#### Auto-Update
- The application automatically refreshes glucose data every 5 minutes (configurable)
- You can manually refresh by clicking the "↻ Refresh" button

#### Settings
Click the "⚙ Settings" button to configure:
- **Update Interval**: Set how often to check for new readings (1-30 minutes)
- **Server Region**: Choose between European or Global LibreView server
- **Notifications**: Enable/disable notifications (future feature)
- **Thresholds**: Set custom high/low glucose alert thresholds

#### Security
- Credentials are encrypted using Windows Data Protection API
- Stored securely in: `%APPDATA%\LibreLinkConnector\credentials.dat`
- Only accessible by the current Windows user
- Can be deleted on logout

## API Information

This application uses the LibreLinkUp API to retrieve glucose data:
- **European Server**: `https://api-eu.libreview.io`
- **Global Server**: `https://api.libreview.io`

### Required Headers
```
product: llu.android
version: 4.2.1
```

### API Flow
1. Login → Get JWT Token
2. Get Connections → Retrieve Patient ID
3. Get Graph Data → Fetch glucose readings

## Configuration Files

The application stores configuration in:
- **Credentials**: `%APPDATA%\LibreLinkConnector\credentials.dat` (encrypted)
- **Settings**: `%APPDATA%\LibreLinkConnector\settings.json`

## Privacy & Security

- All credentials are encrypted using Windows Data Protection API
- No data is sent to third parties
- Direct communication with official LibreView API only
- JWT tokens are stored securely and automatically refreshed

## Troubleshooting

### Login Fails
- Verify your LibreLinkUp credentials
- Check your internet connection
- Try switching server region in Settings

### Auto-Update Not Working
- Check Settings → Update Interval is set correctly
- Ensure the application remains open
- Check if you have active internet connection

### Saved Credentials Not Working
- Try logging out and logging in again
- Delete saved credentials via Logout → Yes

## Project Structure

```
LibreLinkConnector/
├── Models/
│   ├── LoginModels.cs        # Login request/response models
│   ├── ConnectionModels.cs   # Patient connection models
│   ├── GraphModels.cs        # Glucose data models
│   ├── GlucoseData.cs        # Glucose data processing
│   ├── AppState.cs           # Application state
│   └── UserSession.cs        # User session management
├── Services/
│   ├── LibreLinkApiClient.cs # API client implementation
│   ├── CredentialManager.cs  # Secure credential storage
│   ├── ApiLogger.cs          # API logging
│   └── ThemeManager.cs       # Theme management
├── Views/
│   ├── LoginView.xaml        # Login view UI
│   ├── SettingsView.xaml     # Settings view UI
│   ├── ForecastView.xaml     # Forecast view UI
│   └── WidgetView.xaml       # Widget view UI
├── Presenters/
│   ├── LoginPresenter.cs     # Login business logic
│   ├── SettingsPresenter.cs  # Settings business logic
│   └── ForecastPresenter.cs  # Forecast business logic
├── Themes/
│   ├── DarkTheme.xaml        # Dark theme resources
│   └── LightTheme.xaml       # Light theme resources
├── MainWindow.xaml           # Main UI
├── MainWindow.xaml.cs        # Main logic & auto-update
├── AppSettings.cs            # Application settings
├── App.xaml                  # Application resources & styles
└── App.xaml.cs               # Application entry point
```

## Technologies Used

- **WPF** (Windows Presentation Foundation) for UI
- **.NET 8.0** for runtime
- **Newtonsoft.Json** (v13.0.3) for JSON serialization
- **HttpClient** for API communication
- **Windows Data Protection API** for credential encryption

## Building from Source

See [BUILD.md](BUILD.md) for detailed build instructions.

### Quick Build
```powershell
dotnet restore
dotnet build
dotnet run
```

### Create Release Build
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Known Limitations

- Supports only one patient connection at a time
- Requires active internet connection
- Windows-only application

## Roadmap & Future Enhancements

See [CHANGELOG.md](CHANGELOG.md) for planned features and version history.

- [ ] Toast notifications for high/low glucose alerts
- [ ] Multiple patient support
- [ ] Data export to CSV/Excel
- [ ] Enhanced glucose charts and trends
- [ ] System tray support
- [ ] Alarm sounds for critical values

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Credits

- Based on LibreLinkUp HTTP API documentation by Selcuk Kekec
- Built with .NET 8.0 and WPF

## Disclaimer

⚠️ **Important**: This application is not affiliated with or endorsed by Abbott, FreeStyle Libre, or LibreLink. 

- This is an independent, open-source project
- Use at your own risk
- Always consult your healthcare provider for medical decisions
- Please respect LibreView's Terms of Service and API usage policies

## Support

For issues, questions, or feature requests:
1. Check the [Troubleshooting Guide](TROUBLESHOOTING.md)
2. Review [Quick Start Guide](QUICKSTART.md)
3. Open an [Issue](../../issues) on GitHub

## Version

Current version: **1.0.0-beta**

**[📥 Download Latest Release](https://github.com/Pavlo2809/LibreLink-Connector/releases/latest)**

See [CHANGELOG.md](CHANGELOG.md) for version history and release notes.

