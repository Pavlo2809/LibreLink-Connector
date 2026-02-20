# LibreLink Connector - Build Instructions

## Quick Start

### Build and Run

Open PowerShell in the project directory and run:

```powershell
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

### Build Release Version

To create a self-contained executable:

```powershell
# Build for Windows x64
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# The executable will be in:
# bin\Release\net8.0-windows\win-x64\publish\LibreLinkConnector.exe
```

### Build Options

**Framework-dependent (requires .NET 8.0 installed):**
```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

**Self-contained (includes .NET runtime):**
```powershell
dotnet publish -c Release -r win-x64 --self-contained true
```

**Single-file executable:**
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10 or later
- Visual Studio 2022 (optional, for IDE development)

## Development

### Using Visual Studio
1. Open `LibreLinkConnector.csproj` in Visual Studio 2022
2. Press F5 to build and run

### Using VS Code
1. Install C# extension
2. Open the project folder
3. Press F5 or use the Run and Debug panel

### Using Command Line
```powershell
dotnet watch run
```
This will automatically rebuild when you change files.

## Troubleshooting Build Issues

### Missing SDK
```
Error: The Sdk 'Microsoft.NET.Sdk' could not be found
```
**Solution:** Install .NET 8.0 SDK from microsoft.com

### Missing Dependencies
```
Error: Package 'Newtonsoft.Json' is missing
```
**Solution:** Run `dotnet restore`

### Target Framework Error
```
Error: The TargetFramework value 'net8.0-windows' was not recognized
```
**Solution:** Ensure you have .NET 8.0 SDK installed

## Clean Build

To perform a clean build:

```powershell
dotnet clean
dotnet restore
dotnet build
```

## Version Information

- Target Framework: .NET 8.0 Windows
- Language: C# 12
- Platform: Windows x64
- UI Framework: WPF

## Project Dependencies

- Newtonsoft.Json (13.0.3)
- System.Security.Cryptography.ProtectedData (8.0.0)

These are automatically downloaded during `dotnet restore`.
