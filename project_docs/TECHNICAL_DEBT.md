# Technical Debt

- **Localization Storage:** The localization dictionary is hardcoded inside `LocalizationHelper.cs`.
  - *Risk:* Hard to scale for dozens of pages.
  - *Mitigation Plan:* Move keys to standard ASP.NET Core `.resx` resource files.
- **Pass Verification Strength:** Password checks only require a length of 6 characters.
  - *Risk:* Weak passwords allowed.
  - *Mitigation Plan:* Implement full ASP.NET Identity password strength validation rules.
- **Mock Data dependence:** `DbInitializer` recreates or populates the DB on startup if empty.
  - *Risk:* Development changes might trigger seed duplication.
  - *Mitigation Plan:* Refactor seed mechanism to run via separate seed scripts or migrations script.
