# Architecture: OKÜ OBS

## Technology Stack
- **Framework:** ASP.NET Core 9.0 (MVC pattern)
- **Database:** SQLite (`obs.db`)
- **ORM:** Entity Framework Core with Migrations
- **Styling:** Tailwind CSS (loaded via CDN) with custom CSS utility rules
- **Auth:** ASP.NET Core Cookie Authentication
- **Localization:** Custom Cookie/Header-based `LocalizationHelper` supporting TR and EN

## Module Architecture
- **Controllers:** Handle HTTP requests, manage routing, session/authentication, database updates, and return JSON/HTML views.
- **Models:** Entity classes representing database tables and domain properties.
- **Views:** Razor Views (.cshtml) containing responsive layouts, bento box designs, localization strings, and AJAX integration.
- **Data (Migrations & Seeding):** `ApplicationDbContext` mappings and `DbInitializer` for mock data seeding.
