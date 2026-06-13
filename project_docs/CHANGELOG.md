# Changelog

## [2026-06-13]
### Added
- Created `SecurityHelper` providing secure PBKDF2 password hashing utilizing the native ASP.NET Core `PasswordHasher`.
- Integrated `SecurityHelper` into `AuthController`, `AdvisorAuthController`, and `SettingsController` to secure user credentials.
- Created `project_docs/` folder containing standard project specification, rules, memory, tasks, and file structure documentation.
- Integrated `AddUserPreferences` database fields to `ApplicationUser`.
- Created `LocalizationHelper` class for TR/EN translations.
- Wired password change and preference updates endpoints in `SettingsController`.
- Modified layout class/lang attributes to bind to server cookies.

### Fixed
- Fixed side panel contrast conflict with theme variables.
- Terminated running processes on port 5177 to resolve EF Core SQLite lock errors.
