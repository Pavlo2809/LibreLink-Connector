# Quick Start Guide - LibreLink Connector

## 🚀 Get Started in 3 Steps

### Step 1: Build the Application

Open PowerShell in the project folder and run:

```powershell
dotnet restore
dotnet build
```

### Step 2: Run the Application

```powershell
dotnet run
```

Or double-click the executable in:
```
bin\Debug\net8.0-windows\LibreLinkConnector.exe
```

### Step 3: Login

1. Enter your LibreLinkUp email and password
2. Check "Remember me" to save credentials
3. Click "Login"

That's it! Your glucose data will now automatically update every 5 minutes.

---

## 📊 What You'll See

### Main Dashboard
- **Current Glucose Level**: Large display of your most recent reading
- **Trend Arrow**: Shows if glucose is rising (↑), falling (↓), or stable (→)
- **Status**: Color-coded status (Normal/High/Low)
- **Target Ranges**: Your high/low thresholds
- **Recent History**: List of your last 10 readings

### Auto-Update
- Refreshes automatically every 5 minutes
- Manual refresh available via "↻ Refresh" button
- Status bar shows last update time

---

## ⚙️ Configuration

### Change Update Interval

1. Click "⚙ Settings"
2. Adjust the slider (1-30 minutes)
3. Click "Save"

### Switch Server Region

If you're outside Europe:
1. Click "⚙ Settings"
2. Select "Global Server"
3. Click "Save"
4. Logout and login again

### Set Custom Thresholds

1. Click "⚙ Settings"
2. Enter your custom high/low values
3. Click "Save"

---

## 🔒 Security

Your credentials are:
- ✅ Encrypted using Windows Data Protection API
- ✅ Stored locally on your computer only
- ✅ Never shared with third parties
- ✅ Can be deleted via Logout

---

## 🐛 Troubleshooting

### "Login failed"
- Check your LibreLinkUp email/password
- Try switching server region (EU ↔ Global)
- Verify internet connection

### "No patient connections found"
- Ensure you have a LibreLinkUp account with follower access
- Check that someone is sharing their data with you

### Data not updating
- Check your internet connection
- Verify Settings → Update Interval is not 0
- Click "↻ Refresh" to manually update

---

## 📝 First Time Users

### What is LibreLinkUp?
LibreLinkUp is a companion app that allows you to follow someone using a FreeStyle Libre glucose monitor.

### Do I need a sensor?
No! This app is for **followers** - people who want to monitor someone else's glucose data.

### Where do I get login credentials?
1. Download LibreLinkUp app (Android/iOS)
2. Set up your account as a follower
3. Use those same credentials in this app

---

## 🎯 Best Practices

1. **Keep the app running**: Close it to system tray instead of exiting
2. **Regular updates**: 5 minutes is recommended (longer = fewer API calls)
3. **Check history**: Review trends, not just current values
4. **Secure your PC**: Since credentials are saved, ensure your Windows account is password-protected

---

## 💡 Tips

- The app must stay open to auto-update (minimize it to taskbar)
- JWT tokens last ~6 months, so you rarely need to re-login
- "Remember me" is safe - credentials are Windows-encrypted
- Trend arrows update every reading:
  - ↑↑ Rising rapidly (>2 mg/dL/min)
  - ↑ Rising
  - → Stable
  - ↓ Falling
  - ↓↓ Falling rapidly (<-2 mg/dL/min)

---

## 📞 Need Help?

Refer to:
- [README.md](README.md) - Full documentation
- [BUILD.md](BUILD.md) - Build instructions

---

**Ready to start monitoring? Just run `dotnet run` and login!** 🎉
