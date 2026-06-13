# API Specification

## Auth Endpoints
- `GET /Auth/Login` - Render login view
- `POST /Auth/Login` - Authenticate student/advisor
- `GET /Auth/Logout` - Terminate cookie session
- `GET /Auth/Register` - Render registration view
- `POST /Auth/Register` - Create new student account (JSON response)

## Settings Endpoints
- `GET /Settings/Index` - Render settings landing view (Profile, Preferences, Security tabs)
- `POST /Settings/UpdateProfile` - Update student profile (First/Last name, email) (JSON)
- `POST /Settings/UploadProfilePicture` - Upload profile picture file (JSON)
- `POST /Settings/UpdatePassword` - Validate current password and update to new password (JSON)
- `POST /Settings/UpdatePreferences` - Save notification, theme, and language choices (JSON)
