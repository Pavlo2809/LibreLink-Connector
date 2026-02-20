# Troubleshooting Login Issues - LibreLink Connector

## 403 Forbidden Error

If you're getting a **403 Forbidden** error when trying to log in, here are the most common causes and solutions:

### 1. Wrong Server Region ⚠️

**This is the most common issue!**

The LibreLink API has different servers for different regions:
- **European Server**: `https://api-eu.libreview.io`
- **Global Server**: `https://api.libreview.io`

**How to fix:**
1. Click **⚙ Settings** button
2. Try switching between **European Server** and **Global Server**
3. Click **Save**
4. Try logging in again

**How do I know which server to use?**
- If you're in Europe (Germany, UK, France, etc.) → Use European Server
- If you're in US, Australia, or other regions → Try Global Server first
- If unsure, try both!

---

### 2. Incorrect Credentials

**Symptoms:**
- 403 Forbidden error
- "Response status code does not indicate success"

**Solution:**
1. Double-check your **email address** (no typos!)
2. Double-check your **password** (case-sensitive!)
3. Make sure you're using **LibreLinkUp credentials**, not FreeStyle Libre app credentials
4. Try logging into LibreLinkUp mobile app first to verify credentials work

---

### 3. Account Type Issue

**Important:** This app is for **LibreLinkUp followers only**, not for patients with sensors.

**What is LibreLinkUp?**
- LibreLinkUp is a **companion app** that lets you view someone else's glucose data
- You need to have been **invited** to follow a patient
- You cannot use your FreeStyle Libre account directly

**Don't have LibreLinkUp access?**
1. Download LibreLinkUp app (iOS/Android)
2. Create an account
3. Accept an invitation from a FreeStyle Libre user
4. Then use those credentials in this app

---

### 4. Rate Limiting

If you've tried logging in many times:
- Wait 5-10 minutes before trying again
- The API may have temporarily blocked your IP

---

### 5. API Headers Issue

The app now sends these required headers:
```
product: llu.android
version: 4.2.1
User-Agent: LibreLinkConnector/1.0
```

If you still get errors, the API may have changed its requirements.

---

## Testing Steps

Follow these steps in order:

### Step 1: Verify Credentials
1. Open LibreLinkUp mobile app
2. Log in with your credentials
3. Verify you can see glucose data
4. Use the **exact same credentials** in this app

### Step 2: Try Different Server
1. Open Settings (⚙ button)
2. Select "Global Server"
3. Try logging in
4. If that fails, try "European Server"

### Step 3: Check Error Details
The app now shows detailed error messages:
- Look for the HTTP status code (403, 401, etc.)
- Look for any response from the server
- Share this information if you need support

### Step 4: Clear Saved Credentials
1. Click Logout
2. Choose "Yes" to delete saved credentials
3. Restart the application
4. Try logging in fresh

---

## Error Code Reference

| Error Code | Meaning | Solution |
|------------|---------|----------|
| 403 Forbidden | Server rejected request | Wrong server region OR incorrect credentials |
| 401 Unauthorized | Invalid credentials | Check email/password |
| 429 Too Many Requests | Rate limited | Wait 5-10 minutes |
| 500 Internal Server Error | Server problem | Try again later |
| Network error | No internet connection | Check your internet |

---

## Still Not Working?

### Check Network
1. Make sure you have internet connection
2. Try accessing https://api-eu.libreview.io in a browser
3. Check if your firewall is blocking the app

### Verify Account Status
1. Log into https://www.libreview.com
2. Or log into LibreLinkUp mobile app
3. Make sure your account is active and you can see data

### Get More Details
The app now shows detailed error messages including:
- HTTP status code
- Server response (if any)
- Full error text

Copy this information for troubleshooting.

---

## Quick Fix Checklist

- [ ] Tried both European AND Global servers
- [ ] Verified credentials work in LibreLinkUp mobile app
- [ ] Waited 5 minutes if I tried logging in many times
- [ ] Cleared saved credentials and tried fresh
- [ ] Checked internet connection
- [ ] Account has follower access (not just patient account)

---

## Most Likely Solution

**90% of login issues are fixed by changing the server region!**

1. Open Settings
2. Switch from European ↔ Global server
3. Click Save
4. Try logging in again

---

## Example: Working Configuration

**For European users:**
- Server: European Server (api-eu.libreview.io)
- Email: your-email@example.com
- Password: your-password

**For US/Other users:**
- Server: Global Server (api.libreview.io)
- Email: your-email@example.com
- Password: your-password

---

## Contact & Support

If none of these solutions work:

1. Copy the full error message from the app
2. Note which server region you tried
3. Confirm your credentials work in LibreLinkUp mobile app
4. Check the GitHub issues or documentation

**Remember:** This is an unofficial app. If LibreLinkUp API changes, the app may need updates.

---

## Advanced: Manual API Test

To test if the API is responding, open PowerShell and run:

```powershell
# Test European server
Invoke-RestMethod -Uri "https://api-eu.libreview.io/llu/auth/login" `
  -Method POST `
  -Headers @{ "product"="llu.android"; "version"="4.2.1"; "Content-Type"="application/json" } `
  -Body '{"email":"your-email@example.com","password":"your-password"}'

# Test Global server
Invoke-RestMethod -Uri "https://api.libreview.io/llu/auth/login" `
  -Method POST `
  -Headers @{ "product"="llu.android"; "version"="4.2.1"; "Content-Type"="application/json" } `
  -Body '{"email":"your-email@example.com","password":"your-password"}'
```

Replace `your-email@example.com` and `your-password` with your actual credentials.

If this works but the app doesn't, please report the issue!
