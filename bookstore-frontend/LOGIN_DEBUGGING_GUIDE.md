# Login Failure Debugging Guide

## ✅ Fixed Issue

**Problem:** `authApi.js` was creating its own axios instance without token management interceptors.

**Solution:** Updated `authApi.js` to use the configured `api` instance from `axiosConfig.js` which includes:

- Token management
- Automatic token refresh
- Error handling
- CORS headers

## How to Test After Fix

### Step 1: Clear Browser Data

1. Open DevTools (F12)
2. Application → Storage → Clear Site Data
3. Close and reopen browser

### Step 2: Try Login

1. Navigate to login page
2. Enter valid credentials (username + password)
3. Click Login

### Expected Behavior

- Toast notification: "Login successful!"
- Redirected to dashboard/home
- User info displayed in header

## Debugging Checklist

### 1. Check Backend is Running

```bash
# Backend should be running on https://localhost:7069
# Check if you can access: https://localhost:7069/api/Auth/me
# Should return 401 Unauthorized (expected without token)
```

**If backend is not running:**

- Open Visual Studio
- Build the OnlineBookStoreApi project
- Start it with F5 or Ctrl+F5
- Wait for it to be ready

### 2. Check Frontend Environment

```bash
# Verify REACT_APP_API_URL is set
echo $env:REACT_APP_API_URL    # Windows PowerShell
# Or check: bookstore-frontend/.env
```

**Expected output:**

```
https://localhost:7069/api
```

### 3. Browser Console Errors

Open DevTools (F12) and check Console tab:

**Look for:**

- Red error messages
- Network errors (CORS errors)
- 404 or 500 status codes
- POST request to `/Auth/Login`

**Copy full error message if present**

### 4. Check Network Tab

1. Open DevTools → Network tab
2. Clear network log
3. Try to login
4. Look for POST request to `/Auth/Login`

**Successful request should show:**

```
Status: 200 (OK)
Response Headers:
  - Set-Cookie: refreshToken=...;HttpOnly
Response Body:
  {
    "token": "eyJ...",
    "tokenExpiresOn": "2026-02-08T..."
  }
```

**Failed request might show:**

- Status: 401 or 400 (bad credentials)
- Status: 500 (server error)
- Status: 0 (CORS blocked)

### 5. Check Backend Configuration

Verify `appsettings.json` has CORS configured:

```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "https://localhost:3000"]
  }
}
```

Or add if missing:

```json
"AllowedOrigins": ["http://localhost:3000"]
```

### 6. Check Refresh Token Cookie

1. DevTools → Application → Cookies
2. Look for cookie named: `refreshToken`
3. Should have:
   - ✓ HttpOnly flag
   - ✓ Secure flag
   - ✓ SameSite: Strict/Lax

**If not present:**

- Backend is not setting the cookie
- Check `AuthController.SetRefreshTokenCookie()` method
- Verify cookie settings are correct

## Common Error Messages & Solutions

### Error: "Failed to fetch" or "Network Error"

**Cause:** Backend not running or unreachable

**Solution:**

1. Verify backend is running on https://localhost:7069
2. Check HTTPS certificate is trusted
3. Check firewall isn't blocking port 7069
4. Restart backend

### Error: "CORS policy: No 'Access-Control-Allow-Origin' header"

**Cause:** Backend CORS not configured

**Solution:**
In backend `Program.cs`, add CORS:

```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
        builder.WithOrigins("http://localhost:3000", "https://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});
```

Then in middleware:

```csharp
app.UseCors("AllowFrontend");
```

### Error: "Invalid username or password"

**Cause:** Wrong credentials or user doesn't exist

**Solution:**

1. Verify username and password are correct
2. Check database has the user
3. Verify password in database (check hash)
4. Try registering new account if needed

### Error: "TypeError: authApi.login is not a function"

**Cause:** authApi export is wrong

**Solution:**
Verify `authApi.js` exports correctly:

```javascript
export const authApi = {
  login: (credentials) => api.post("/Auth/Login", credentials),
  // ...
};
```

### Error: "Cannot read property 'data' of undefined"

**Cause:** Response doesn't match expected format

**Solution:**
Check backend returns:

```json
{
  "token": "string",
  "tokenExpiresOn": "datetime"
}
```

### Error: "Token lost after page refresh"

**Expected behavior** (not an error):

- This is normal - tokens are memory-only
- Session restored automatically via refresh token
- Token will be restored on next API call
- If not restored, refresh token cookie is invalid

### Error: "Infinite redirect to /login"

**Cause:** Token refresh failing

**Solution:**

1. Check /Auth/RefreshToken endpoint works
2. Verify refresh token cookie is set
3. Check backend token validation logic
4. Clear cookies and try login again

## Step-by-Step Login Flow Verification

### 1. Frontend Calls Login

```javascript
const response = await authApi.login({
  username: "testuser",
  password: "password123",
});
```

**Check in DevTools:**

- Network tab should show POST to `/Auth/Login`
- Request body: `{"username":"testuser","password":"password123"}`

### 2. Backend Validates Credentials

**Check backend logs:**

- Should show login attempt
- Should show user found or not found

### 3. Backend Returns Token

**Check Network Response:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenExpiresOn": "2026-02-08T15:30:00Z"
}
```

**Also check Response Headers:**

```
Set-Cookie: refreshToken=...;Path=/;HttpOnly;Secure;SameSite=Strict
```

### 4. Frontend Stores Token

**Check In Memory** (can't directly verify):

- Token is stored via `tokenManager.setToken()`
- Should be invisible in DevTools

### 5. Frontend Fetches User Info

**Check Network:**

- POST to `/Auth/me` should be made
- Request should have `Authorization: Bearer {token}` header

### 6. Frontend Stores User & Redirects

**Check DevTools → Storage:**

- `localStorage['user']` should contain user object with role

**Check Page:**

- Should redirect to dashboard (admin) or home (customer)

## Testing Credentials

### Register New User First

If login fails with "Invalid username or password":

1. Go to Register page
2. Fill in form:
   - Username: testuser
   - Password: Test123!
   - Email: test@example.com
   - First Name: Test
   - Last Name: User
3. Click Register
4. Should redirect to dashboard
5. Logout and try login with same credentials

## Advanced Debugging

### Enable Browser DevTools Network Logging

```javascript
// In browser console
localStorage.setItem("debugAuth", "true");
// Reload page and check console for auth logs
```

### Check TokenManager Works

```javascript
// In browser console
import { getToken, setToken } from "./api/tokenManager.js";
setToken("test-token", "2026-12-31T23:59:59Z");
console.log(getToken()); // Should output: test-token
```

### Check AxiosConfig Interceptors

```javascript
// In browser console
// Make a test request to verify interceptors work
fetch("/api/Auth/me")
  .then((r) => r.json())
  .then(console.log)
  .catch(console.error);
```

## Still Not Working?

### Collect Debug Information

1. Full error message from browser console
2. Network tab screenshot (POST to /Auth/Login)
3. Response headers from /Auth/Login request
4. Response body from /Auth/Login request
5. Backend error logs
6. Check .env file has correct API URL

### Test Backend Directly

```bash
# Window PowerShell
$body = @{
  username = "testuser"
  password = "password123"
} | ConvertTo-Json

Invoke-WebRequest -Uri "https://localhost:7069/api/Auth/Login" `
  -Method Post `
  -Body $body `
  -ContentType "application/json"
```

If this command fails:

- Backend is not running properly
- HTTPS certificate not trusted
- Port 7069 not open

### Reset Everything

1. Stop frontend: Ctrl+C in terminal
2. Stop backend: Stop debug session
3. Clear browser: DevTools → Storage → Clear All
4. Delete node_modules: `rm -r node_modules`
5. Reinstall: `npm install`
6. Start backend first
7. Start frontend: `npm start`
8. Try login again

## Performance Check

If login is slow:

1. Check Network tab for slow requests
2. Verify /Auth/me endpoint performance
3. Check /Auth/RefreshToken latency
4. Look for database query slowness

## Next Steps

1. ✅ Try login with fixed authApi.js
2. If still failing, check browser console for errors
3. Verify backend logs for error details
4. Check network tab to see actual API behavior
5. Verify CORS configuration on backend
6. Test backend endpoint directly with Postman/Thunder Client

Got any specific error message? Share it and I can provide targeted solution!
