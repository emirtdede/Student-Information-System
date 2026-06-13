# Architectural Decisions (ADR)

## ADR 1: SQLite for Local Development database
- **Context:** The system needs a simple, portable relational database.
- **Decision:** SQLite was selected for rapid development without setting up local SQL Server.
- **Consequences:** Simple backups and migrations, but lacks concurrent write throughput scaling (handled via EF Core lock checks).

## ADR 2: Cookie-based Theme & Language settings
- **Context:** Storing settings in `localStorage` caused theme/text flickering during HTML load.
- **Decision:** Stored values both in DB and client cookies, allowing the server to output correctly structured `<html>` class and lang attributes directly.
- **Consequences:** Flicker-free loading and immediate localization application.
