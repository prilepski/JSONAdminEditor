# Frontend Deployment Requirements - JSON Admin Editor

**From: Frontend Lead**  
**To: DevOps Team**  
**Subject: React Frontend Deployment Requirements for Dev Environment**

## Application Overview

This is a React 18 + TypeScript SPA built with Vite. The application manages JSON configurations and requires Okta SSO authentication.

## Technical Stack

- **Framework**: React 18 with TypeScript
- **Build Tool**: Vite 7.x
- **Package Manager**: PNPM (required - do not use npm/yarn)
- **Node.js**: 18+ (LTS recommended)
- **Output**: Static files in `dist/` folder

## Repository Information

- **Main Branch**: `main` (production)
- **Dev Branch**: `develop` (development environment)
- **Build Command**: `pnpm build:dev`
- **Dependencies**: Managed via `pnpm-lock.yaml`

## Environment Variables Required

The application reads environment variables from `process.env` (not from .env files in deployed environments).

### Required Variables:
```bash
VITE_API_BASE_URL=https://dev-api.company.com/api
VITE_APP_NAME="JSON Admin Editor (Dev)"
VITE_APP_ENV=development
VITE_ENABLE_DEVTOOLS=true
VITE_SOURCEMAP=true
```

### Okta SSO Variables (Sensitive):
```bash
VITE_OKTA_DOMAIN=dev-company.okta.com
VITE_OKTA_CLIENT_ID=<from-okta-application>
VITE_OKTA_REDIRECT_URI=https://dev-app.company.com/login/callback
VITE_OKTA_SCOPES="openid profile email"
VITE_OKTA_RESPONSE_TYPE=code
VITE_OKTA_GRANT_TYPE=authorization_code
```

**Note**: Store Okta variables in your secrets management system.

## Build Process

### Prerequisites:
```bash
# Install Node.js 18+
node --version  # Must be 18+

# Install PNPM globally
npm install -g pnpm

# Verify PNPM
pnpm --version
```

### Build Steps:
```bash
# 1. Clone repository
git clone <repo-url>
cd react-client

# 2. Install dependencies (uses pnpm-lock.yaml)
pnpm install

# 3. Set environment variables (see above)
export VITE_API_BASE_URL=...
export VITE_OKTA_DOMAIN=...
# ... other variables

# 4. Build application
pnpm build:dev

# 5. Verify output
ls -la dist/
# Should contain: index.html, assets/ folder with JS/CSS files
```

## Web Server Configuration

### Requirements:
- **SPA Routing**: All routes must serve `index.html`
- **API Proxy**: `/api/*` requests proxy to backend
- **Static Assets**: Serve from `dist/` folder
- **HTTPS**: Required for Okta authentication

### Nginx Configuration:
```nginx
server {
    listen 443 ssl;
    server_name dev-app.company.com;
    root /path/to/dist;
    index index.html;

    # SPA routing - CRITICAL
    location / {
        try_files $uri $uri/ /index.html;
    }

    # API proxy - REQUIRED
    location /api/ {
        proxy_pass https://dev-api.company.com/api/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Static assets caching
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

## Backend Dependencies

### API Requirements:
- **Backend URL**: Must be accessible at `https://dev-api.company.com/api`
- **CORS Headers**: Must allow requests from `https://dev-app.company.com`
- **Endpoints**: All `/api/*` routes must be functional

### Required CORS Headers (Backend Team):
```
Access-Control-Allow-Origin: https://dev-app.company.com
Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
Access-Control-Allow-Headers: Content-Type, Authorization, X-Okta-User
```

## Okta Configuration Requirements

### Application Setup (Identity Team):
- **Application Type**: Single Page App (SPA)
- **Grant Types**: Authorization Code + PKCE
- **Redirect URIs**: `https://dev-app.company.com/login/callback`
- **Logout URIs**: `https://dev-app.company.com`
- **CORS Origins**: `https://dev-app.company.com`

### Network Requirements:
- **Outbound HTTPS**: Allow connections to `*.okta.com` on port 443
- **Domain**: Your specific Okta domain (e.g., `dev-company.okta.com`)

## Health Checks

### Application Health:
```bash
# Frontend accessible
curl -f https://dev-app.company.com/

# API proxy working
curl -f https://dev-app.company.com/api/dictionaries/customers

# Static assets loading
curl -f https://dev-app.company.com/assets/index-*.js
```

### Expected Responses:
- **Frontend**: Returns HTML with React app
- **API**: Returns JSON data (may require authentication)
- **Assets**: Returns JS/CSS files with proper content-type

## CI/CD Integration

### Build Command:
```bash
pnpm build:dev
```

### Output:
- **Directory**: `dist/`
- **Size**: ~2-3MB total
- **Files**: `index.html` + `assets/` folder

### Deployment:
1. Copy `dist/` contents to web server document root
2. Restart/reload web server
3. Verify health checks

## Troubleshooting

### Common Issues:

**Build Fails:**
- Check Node.js version (must be 18+)
- Verify PNPM is installed
- Check environment variables are set

**App Loads but API Fails:**
- Verify backend is running
- Check API proxy configuration
- Verify CORS headers on backend

**Okta Login Fails:**
- Check Okta environment variables
- Verify redirect URIs in Okta app
- Ensure HTTPS is enabled

**Routing Issues (404 on refresh):**
- Verify SPA routing configuration
- Check `try_files` directive in Nginx

### Logs to Check:
- **Build Logs**: Check for TypeScript/build errors
- **Browser Console**: Check for JavaScript errors
- **Network Tab**: Check for failed API calls
- **Web Server Logs**: Check for 404/500 errors

## Performance Expectations

- **Bundle Size**: ~500KB gzipped
- **Load Time**: <2s on fast connection
- **Build Time**: ~30-60 seconds
- **Memory Usage**: ~50MB typical

## Security Considerations

- **HTTPS Required**: Okta authentication requires HTTPS
- **CSP Headers**: Consider Content Security Policy
- **Secrets**: Never expose Okta client secrets (we use PKCE)
- **Environment Variables**: Secure storage for sensitive values

## Support & Escalation

For frontend-related deployment issues:

1. **Build Issues**: Check Node.js/PNPM versions and environment variables
2. **Runtime Issues**: Check browser console and network requests
3. **Authentication Issues**: Verify Okta configuration and HTTPS
4. **API Issues**: Coordinate with backend team for CORS/endpoint issues

**Frontend Team Contact**: [Your contact information]

---

**Note**: This application uses modern React patterns and requires the exact versions specified. Please do not substitute with older versions of Node.js or different package managers.