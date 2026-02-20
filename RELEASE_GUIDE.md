# Release Guide - LibreLink Connector

This guide explains how to create and publish releases for LibreLink Connector.

## 🚀 Creating a New Release

### Step 1: Build the Release

```powershell
# Clean previous builds
dotnet clean "LibreLinkConnector.csproj" -c Release

# Build self-contained release for Windows x64
dotnet publish "LibreLinkConnector.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ".\release\v1.0.0"

# Create a zip file for distribution
Compress-Archive -Path ".\release\v1.0.0\*" -DestinationPath ".\release\LibreLinkConnector-v1.0.0-win-x64.zip" -Force
```

### Step 2: Update Version Information

Before building, update these files:

1. **LibreLinkConnector.csproj** - Update `<Version>` tag
2. **CHANGELOG.md** - Add new version section with changes
3. **README.md** - Update version badge/info if applicable

### Step 3: Commit and Tag

```powershell
# Commit version changes
git add LibreLinkConnector.csproj CHANGELOG.md
git commit -m "Bump version to 1.0.1"

# Create and push tag
git tag -a v1.0.1 -m "Release version 1.0.1"
git push origin main
git push origin v1.0.1
```

### Step 4: Create GitHub Release

1. Go to your repository on GitHub
2. Click **Releases** → **Draft a new release**
3. Fill in the release information:

   **Choose a tag:** `v1.0.0` (or create new)
   
   **Release title:** `LibreLink Connector v1.0.0`
   
   **Description:** Copy from CHANGELOG.md or write:
   ```markdown
   ## What's New in v1.0.0
   
   ### Features
   - Initial release
   - Secure LibreLinkUp API integration
   - Auto-update functionality
   - Modern WPF interface
   
   ### Requirements
   - Windows 10 or later
   - .NET 8.0 Runtime (included in self-contained build)
   
   ### Installation
   1. Download `LibreLinkConnector-v1.0.0-win-x64.zip`
   2. Extract to your desired location
   3. Run `LibreLinkConnector.exe`
   
   See [README.md](README.md) for more details.
   ```

4. **Attach files:**
   - Drag & drop: `.\release\LibreLinkConnector-v1.0.0-win-x64.zip`

5. Check **Set as the latest release**

6. Click **Publish release**

## 📦 Release Checklist

Before creating a release:

- [ ] All tests pass (if applicable)
- [ ] Version number updated in `.csproj`
- [ ] CHANGELOG.md updated with new version
- [ ] README.md updated if needed
- [ ] Build successful without errors
- [ ] Tested the release build locally
- [ ] All changes committed to git
- [ ] Tagged with version number

## 🏷️ Version Numbering

Follow [Semantic Versioning](https://semver.org/):

- **MAJOR.MINOR.PATCH** (e.g., 1.0.0)
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

Examples:
- `1.0.0` - Initial release
- `1.0.1` - Bug fixes
- `1.1.0` - New features
- `2.0.0` - Breaking changes

## 📋 Build Variants

### Self-Contained (Recommended for end users)
Includes .NET runtime (~150 MB)
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Framework-Dependent (Smaller size)
Requires .NET 8.0 installed (~20 MB)
```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

### Multiple Platforms
```powershell
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained true -o ".\release\win-x64"

# Windows x86
dotnet publish -c Release -r win-x86 --self-contained true -o ".\release\win-x86"

# Windows ARM64
dotnet publish -c Release -r win-arm64 --self-contained true -o ".\release\win-arm64"
```

## 🔄 Release Automation (Future)

Consider setting up GitHub Actions for automated releases:

```yaml
# .github/workflows/release.yml
name: Release

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      - name: Publish
        run: dotnet publish -c Release -r win-x64 --self-contained true
      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: release/**/*.zip
```

## 📊 Current Release

**Version:** 1.0.0  
**Date:** February 20, 2026  
**Build Location:** `.\release\v1.0.0\`  
**Zip File:** `.\release\LibreLinkConnector-v1.0.0-win-x64.zip`

## 🎯 Upload Your First Release

Your v1.0.0 release is ready to upload! The zip file is at:
```
.\release\LibreLinkConnector-v1.0.0-win-x64.zip
```

Follow Step 4 above to create your GitHub Release and upload this file.

---

**Note:** The `release/` folder is in `.gitignore` - releases are distributed via GitHub Releases, not committed to the repository.
