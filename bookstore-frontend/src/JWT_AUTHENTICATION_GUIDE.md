# Frontend JWT Authentication Implementation Guide

## Overview

The frontend has been updated to work with the new JWT-based authentication system from the backend. This document explains the authentication flow, token management, and how to use the authentication system.

**SECURITY NOTE:** This implementation includes critical XSS protection by storing JWT tokens in memory only, not in localStorage. See "XSS Protection" section below for details.

## Architecture

### Key Components

#### 1. **authApi.js** - Authentication API Endpoints

Located at `src/api/authApi.js`, this module handles all authentication-related API calls:

```javascript
-POST / Auth / Login - // Login with username/password
  POST / Auth / Register - // Register new user
  POST / Auth / RefreshToken - // Refresh expired JWT token
  POST / Auth / Logout - // Logout and revoke tokens
  GET / Auth / me; // Get current user info
```

**Features:**

- Includes `withCredentials: true` to handle HTTP-only refresh token cookies
- Automatically included in axios instance for all auth endpoints

#### 2. **axiosConfig.js** - HTTP Client with Token Management

Located at `src/api/axiosConfig.js`, this is the main axios instance with interceptors.

**Request Interceptor:**

- Automatically adds `Authorization: Bearer {jwtToken}` header to all requests
- Retrieves token from `localStorage.jwtToken`

**Response Interceptor:**

- Handles 401 (Unauthorized) errors by automatically refreshing the JWT token
- Implements a token refresh queue to prevent multiple simultaneous refresh calls
- On successful token refresh, updates the token in localStorage and retries the original request
- On refresh failure, clears all auth data and redirects to login page

**Token Refresh Strategy:**

- Uses a queue-based approach (`isRefreshing` and `failedQueue`) to handle concurrent requests
- Only one token refresh happens at a time
- All failed requests during refresh are queued and retried after token refresh completes

#### 3. **AuthContext.jsx** - Authentication State Management

Located at `src/context/AuthContext.jsx`, this React context manages global authentication state.

**State Variables:**

- `user` - Current logged-in user object
- `loading` - Loading state during initialization
- `isAuthenticated` - Boolean flag indicating if user is authenticated

#### 4. **tokenManager.js** - Secure In-Memory Token Storage

Located at `src/api/tokenManager.js`, this module manages JWT tokens in memory.

**Functions:**

- `setToken(token, expiresOn)` - Store token in memory
- `getToken()` - Retrieve token from memory
- `hasValidToken()` - Check if valid token exists
- `clearToken()` - Remove token from memory
- `subscribeToTokenChanges(listener)` - Listen for token changes

**Security Features:**

- Tokens stored only in JavaScript variable (not in storage APIs)
- Multiple subscribers can listen for token changes
- Tokens completely cleared from memory on logout
- No persistence across page refreshes (intentional)

**Context Methods:**

**login(credentials):**

```javascript
- Calls authApi.login() with username and password
- Stores JWT token in MEMORY ONLY using tokenManager.setToken()
- NOT stored in localStorage (XSS protection)
- Refresh token stored in HTTP-only cookie (by backend)
- Fetches user info from /Auth/me endpoint
- Stores user data in localStorage.user (non-sensitive)
- Sets isAuthenticated to true
- Returns user object
```

**register(data):**

```javascript
- Calls authApi.register() with user registration data
- Same token storage as login (memory-only)
- Automatically logs in the user
- Returns user object
```

**logout():**

```javascript
- Calls authApi.logout() to revoke tokens server-side
- Clears token from memory using tokenManager.clearToken()
- Clears all local auth data even if API call fails
- Removes user from localStorage.user
- Sets isAuthenticated to false
- Displays success toast message
```

**Initialization:**

- On component mount, attempts to refresh token using refresh token cookie
- If refresh succeeds, stores new JWT in memory and restores user session
- If refresh fails (cookie expired), clears all auth data
- Handles XSS scenarios where page is refreshed - token must be re-obtained from refresh cookie

## Token Storage Strategy

### JWT Access Token (CRITICAL SECURITY FIX)

- **Storage Location:** **Memory only** (JavaScript variable in tokenManager module)
- **NOT stored in localStorage or sessionStorage** to prevent XSS theft
- **Lifespan:** Configured in backend appsettings.json (default: 15 minutes)
- **Usage:** Automatically sent with every API request by axios interceptor
- **Lifecycle:** Lost on page refresh (re-obtained via refresh token cookie)
- **Why Memory-Only?** Protects against XSS attacks where malicious scripts cannot access tokens from DOM or storage APIs

### Refresh Token

- **Storage Location:** HTTP-only cookie (managed by browser, not accessible to JavaScript)
- **Lifespan:** Configured in backend appsettings.json (default: 7 days)
- **Purpose:** Used to obtain new JWT tokens when they expire
- **Automatic Handling:** No frontend code needed - axios interceptor handles refresh automatically
- **Cannot be stolen by:** XSS attacks, malicious JavaScript, or DOM inspection

### User Data

- **Storage Location:** `localStorage.user`
- **Content:** User object from /Auth/me endpoint (contains user_id, username, email, role, etc.)
- **Purpose:** Display user info in UI, determine role-based access
- **Risk Level:** Low - contains only non-sensitive metadata (no credentials or tokens)

## Authentication Flow

### Session Persistence with Memory-Only Tokens

**Challenge:** How to maintain session across page refresh when token is only in memory?

**Solution:** Use refresh token cookie for session recovery

```
1. User logs in → JWT stored in memory, refresh token in cookie
2. User refreshes page → JWT lost from memory
3. App initializes → Detects refresh token cookie exists
4. App calls /Auth/RefreshToken → Gets new JWT using refresh cookie
5. New JWT stored in memory → Session restored seamlessly
6. User doesn't notice page refresh (automatic session recovery)
```

**Security Benefits:**

- XSS attacker cannot steal token from memory on initial login
- Token lost on page refresh (XSS attacker must refresh page to get access)
- Refresh token remains secure in HTTP-only cookie
- Automatic logout after refresh token expiration (7 days by default)

### Login Flow

```
1. User enters credentials on LoginPage
2. LoginPage calls useAuth().login(credentials)
3. AuthContext calls authApi.login(credentials)
4. Backend validates credentials and returns JWT token and refresh token
5. Token stored in MEMORY ONLY using tokenManager.setToken()
6. Refresh token stored in HTTP-only cookie by backend
7. User info fetched from /Auth/me endpoint
8. User data stored in localStorage.user (non-sensitive)
9. isAuthenticated set to true
10. User redirected to dashboard/home based on role
```

### Registration Flow

```
1. User fills registration form on RegisterPage
2. RegisterPage calls useAuth().register(data)
3. AuthContext calls authApi.register(data)
4. Backend creates user and returns JWT token and refresh token
5. Token stored in MEMORY ONLY using tokenManager.setToken()
6. Refresh token stored in HTTP-only cookie by backend
7. User info fetched from /Auth/me endpoint
8. User data stored in localStorage.user (non-sensitive)
9. isAuthenticated set to true
10. User redirected to dashboard/home based on role
```

### Protected API Request Flow

```
1. Component calls any API endpoint (e.g., userApi.getProfile())
2. Request interceptor adds Authorization header with JWT token
3. Backend validates JWT token signature and expiration
4. If valid, request proceeds
5. If invalid, backend returns 401 Unauthorized
```

### Token Refresh Flow

```
1. API request made with expired JWT token
2. Backend returns 401 Unauthorized
3. Response interceptor detects 401 error
4. If not already refreshing, calls /Auth/RefreshToken endpoint
5. Backend validates refresh token cookie
6. Backend returns new JWT token
7. New token stored in MEMORY using tokenManager.setToken()
8. NOT stored in localStorage (XSS protection)
9. Original failed request retried with new token
10. If refresh fails, user redirected to login page
11. Token cleared from memory and user logged out
```

### Logout Flow

```
1. User clicks logout button
2. AuthContext calls authApi.logout()
3. Backend revokes all refresh tokens for user
4. Backend removes refresh token cookie
5. Frontend clears localStorage (jwtToken, tokenExpiresOn, user)
6. isAuthenticated set to false
7. User redirected to login page
```

## Usage in Components

### Using Authentication in Components

```javascript
import { useAuth } from "../context/AuthContext";

function MyComponent() {
  const { user, isAuthenticated, login, logout } = useAuth();

  if (!isAuthenticated) {
    return <p>Please log in</p>;
  }

  return (
    <div>
      <p>Welcome, {user.username}!</p>
      <button onClick={logout}>Logout</button>
    </div>
  );
}
```

### Accessing Token in Custom Code (If Needed)

**Generally, you should NOT need to access the token directly** (axios interceptor handles it).

For rare cases where you need the token:

```javascript
import { getToken, hasValidToken } from "../api/tokenManager";

// Check if valid token exists
if (hasValidToken()) {
  const token = getToken();
  // Use token for custom logic
}

// DO NOT store token in localStorage, component state, or props
// DO NOT send token outside secure HTTPS connections
```

### Protecting Routes

Routes are protected using the `ProtectedRoute` component:

```javascript
<Route path="/cart" element={<ProtectedRoute><CartPage /></ProtectedRoute>} />
<Route path="/admin/dashboard" element={<ProtectedRoute adminOnly><AdminDashboard /></ProtectedRoute>} />
```

### Checking User Role

```javascript
const { isAdmin, isCustomer } = useAuth();

if (isAdmin()) {
  // Show admin features
}
```

## Security Features

### XSS (Cross-Site Scripting) Protection - CRITICAL

- **Problem:** Storing tokens in localStorage makes them vulnerable to XSS attacks
  - Malicious scripts can access `localStorage` and steal authentication tokens
  - Attacker gains full access to user account without password
- **Solution:** Store JWT token in memory only (JavaScript variable)
  - Token stored only in RAM, not persisted to any storage API
  - Inaccessible to malicious scripts via DOM or Storage APIs
  - Token lost on page refresh by design (security tradeoff)
  - User session restored automatically via refresh token cookie
- **Why This Works:**
  - Even if XSS vulnerability exists, attacker cannot access memory-stored token
  - Attack surface reduced significantly
  - Refresh token remains secure in HTTP-only cookie (inaccessible to JavaScript)

### HTTP-Only Cookie Security

- Refresh token stored in HTTP-only cookie
- Cannot be accessed by JavaScript (not even by your own code)
- Protected from XSS attacks
- Automatically sent with requests (browser handles this)
- Cannot be read or modified by Document.cookie

### CSRF Protection

- `withCredentials: true` ensures cookies sent with cross-origin requests
- Backend validates CSRF tokens (if configured)

### Token Expiration

- JWT tokens expire after configured duration (default: 15 minutes)
- Automatic refresh happens transparently to user
- Old tokens on other devices become invalid on logout

### Multi-Device Support

- Each login creates a new refresh token
- Old tokens remain valid on other devices
- Logout revokes all tokens (all devices logged out)

## Error Handling

### Login Failures

- Invalid credentials return 400 Bad Request
- Error message displayed via toast notification
- User remains on login page

### Protected Route Access Failures

- 401 Unauthorized on protected route without valid token
- User redirected to login page
- Auth data cleared from localStorage

### Network Failures

- Fetch/network errors displayed via toast
- User can retry or navigate away
- No automatic redirect on network errors

## Backend Integration Points

### Required Backend Endpoints

1. **POST /api/Auth/Login**
   - Body: `{ username, password }`
   - Response: `{ token, tokenExpiresOn }`
   - Cookie: `refreshToken` (HTTP-only)

2. **POST /api/Auth/Register**
   - Body: `{ username, password, email, firstName, lastName, ... }`
   - Response: `{ token, tokenExpiresOn }`
   - Cookie: `refreshToken` (HTTP-only)

3. **GET /api/Auth/me**
   - Headers: `Authorization: Bearer {token}`
   - Response: User object with `user_id`, `username`, `role`, etc.

4. **POST /api/Auth/RefreshToken**
   - Headers: None required (uses cookie)
   - Cookie: `refreshToken` (HTTP-only)
   - Response: `{ token, tokenExpiresOn }`
   - Cookie: New `refreshToken` (HTTP-only)

5. **POST /api/Auth/Logout**
   - Headers: `Authorization: Bearer {token}`
   - Response: Success message
   - Cookie: `refreshToken` deleted

## Environment Variables

Required in `.env` file:

```
REACT_APP_API_URL=https://localhost:7069/api
```

## Troubleshooting

### Issue: "Authentication required" error on all requests

**Cause:** JWT token not available in memory
**Solution:**

- Check network tab to see if /Auth/RefreshToken call succeeds on login
- Check browser console for refresh token errors
- Verify refresh token cookie is set by backend (look in DevTools → Application → Cookies)

### Issue: Session lost after page refresh

**Expected behavior:** This is normal with memory-only tokens
**Solution:**

- Session is automatically restored via refresh token cookie
- If you see 401 error after refresh, refresh token may have expired
- Check browser console for /Auth/RefreshToken errors
- Try logging in again

### Issue: Infinite redirect loop between pages

**Cause:** Token refresh failing repeatedly
**Solution:**

- Check if refresh token cookie exists and is valid
- Check backend logs for refresh token errors
- Clear browser cookies and try logging in again
- Verify refresh token hasn't expired (default 7 days)

### Issue: User data not loading after login

**Cause:** /Auth/me endpoint not returning user data
**Solution:** Verify backend /Auth/me endpoint implementation

### Issue: Logout not working

**Cause:** Logout API call failing
**Solution:** Logout still clears local state even if API fails (by design)

### Recommended Browser DevTools Checks

1. **Application → Storage → Cookies:** Should see `refreshToken` (HTTP-only)
2. **Network tab:** Looking for `/Auth/RefreshToken` calls
3. **Console:** Check for axios errors on 401 responses
4. **localStorage:** Should only contain `user` object, NOT `jwtToken`

## Next Steps / Improvements

### Known Limitations of Memory-Only Token Strategy

1. **Token lost on page refresh** (intentional for security)
   - Mitigation: Automatic restoration via refresh token cookie
   - User doesn't notice - happens transparently

2. **Cannot access token in non-interceptor code**
   - If you need token outside axios requests, use tokenManager.getToken()
   - Most use cases don't need direct token access

### Future Enhancements

1. **Add Token Expiration Warning:** Warn user before token expires (5 minutes before)
2. **Implement "Remember Me":** Option to extend refresh token lifespan
3. **Add OAuth Integration:** Support Google, GitHub, Microsoft login
4. **Token Rotation:** Implement automatic token rotation before expiration
5. **Biometric Authentication:** Add fingerprint/face unlock support
6. **Two-Factor Authentication:** Implement 2FA for enhanced security
7. **Service Worker Caching:** Use Service Worker for improved uptime (advanced)

## References

- JWT Documentation: https://jwt.io
- React Context API: https://react.dev/reference/react/useContext
- Axios Interceptors: https://axios-http.com/docs/interceptors
- HTTP-Only Cookies: https://cheatsheetseries.owasp.org/cheatsheets/Cross-Site_Request_Forgery_Prevention_Cheat_Sheet.html
