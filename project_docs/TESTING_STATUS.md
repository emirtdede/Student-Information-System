# Testing Status

## Automated Tests
- Build verification: Passed (`dotnet build` succeeds with 0 errors and 0 warnings).

## Manual Testing Cases

- **TC1: Login Validation**
  - **Status:** Passed
  - **Description:** Attempting to log in with an invalid student number, email, or password returns a localized error message (TR/EN) matching the current language preference.

- **TC2: Password Update**
  - **Status:** Passed
  - **Description:** Verifies that changing a password enforces minimum length (6 characters), requires verifying the current password correctly, and ensures new passwords match before executing the database update.

- **TC3: Preferences Sync**
  - **Status:** Passed
  - **Description:** Changing theme preference (light/dark), language choice, or notification switches immediately saves to the database and cookie store, causing layout elements to adjust without page flicker.

- **TC4: Hashed Password Storage**
  - **Status:** Passed
  - **Description:** New user registrations and updated passwords are secure. They are stored as salted PBKDF2 hashes instead of plain text. Backward compatibility is maintained for existing plaintext seed data.

- **TC5: Advisor Authentication**
  - **Status:** Passed
  - **Description:** Advisor login validation correctly queries advisor table records and applies identical cryptographic password verification rules.
