# File Structure Overview

```
Student-Information-System/
├── Controllers/                 # ASP.NET Core MVC Controllers
│   ├── AuthController.cs        # Handle student/visitor auth flows
│   ├── SettingsController.cs    # Manage profile and preferences updates
│   └── ...
├── Data/                        # Data mappings & Database initialization
│   ├── ApplicationDbContext.cs  # EF Core DbContext definition
│   ├── DbInitializer.cs         # Mock data population
│   └── LocalizationHelper.cs    # Multi-language text provider
├── Migrations/                  # EF Core Database Migrations
├── Models/                      # Model definitions (Student, Advisor, User, etc.)
├── project_docs/                # Project specifications & Vibe Coding logs
├── Properties/
├── Views/                       # Razor Views (.cshtml templates)
│   ├── Settings/
│   │   ├── Index.cshtml         # Settings tabbed controls & forms
│   │   └── Profile.cshtml
│   ├── Shared/
│   │   └── _Layout.cshtml       # Global layout & bento header/navigation
│   └── ...
├── wwwroot/                     # Static assets (images, css, js)
├── obs.db                       # Local SQLite Database file
├── Program.cs                   # App initialization & middleware pipeline
└── Student-Information-System.csproj
```
