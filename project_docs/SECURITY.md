# Security Specifications

## Authentication
- Handled via `Microsoft.AspNetCore.Authentication.Cookies` middleware.
- Login paths configured dynamically in `Program.cs`.
- Logged-in credentials validated against the `Users` database table.

## Data Protection
- *Current Status:* Development mock password storage.
- *Requirement:* All production user passwords must be hashed before storage.
- *Input Validation:* HTML inputs are sanitized on post requests and checked for emptiness/length constraints.
- *Localization:* Custom Cookie handling uses standard `Lax` SameSite protection.
