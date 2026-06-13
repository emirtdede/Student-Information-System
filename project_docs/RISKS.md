# Risks Registry

- **Risk 1: Database Locks**
  - *Description:* SQLite database locks when concurrent migration and write processes happen.
  - *Impact:* Medium (blocks development server builds).
  - *Mitigation:* Explicitly terminate any running instances of `Student-Information-System.exe` before rebuilding or migrating.

- **Risk 2: Hardcoded Mock Password storage**
  - *Description:* DB currently stores passwords in plain text.
  - *Impact:* High (security risk).
  - *Mitigation:* Apply hashing (`IPasswordHasher` or BCrypt) before production release.
