# Security Rules
## Purpose
To ensure all code is scanned for vulnerabilities and written with security in mind.

## Priority
Critical

## Instructions
-   **Scan code for vulnerabilities**: Any code generated or modified should be scanned for common security flaws like SQL injection or cross-site scripting (XSS).
-   **Validate all inputs**: Always validate and sanitize all user input to prevent injection attacks.
-   **Never expose sensitive information**: Do not write code that logs, returns, or otherwise exposes personally identifiable information (PII) or other sensitive data.
-   **Detect secrets**: Actively scan for and prevent the introduction of hardcoded secrets into the codebase.
