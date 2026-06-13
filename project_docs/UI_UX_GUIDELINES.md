# UI/UX Guidelines

## Colors & Themes
- Curated palette using CSS custom variables matching Tailwind config.
- **Primary Color:** `#00647e` (light mode), `#6cd3fa` (dark mode)
- **Secondary Color:** `#006c49` (light mode), `#4edea3` (dark mode)
- **Surface Panels:** Translucent glassmorphism containers (`glass-panel`) with blur filters.
- **Transitions:** Smooth hover effects, active state micro-scaling, and side panel animations.

## Bento Layout & Responsiveness
- Dashboards utilize a responsive grid structure (`grid grid-cols-1 lg:grid-cols-12 gap-md`).
- High information density with clean spacing and harmonized text sizes (Inter font family).

## User Interactions
- Form validation errors and operation outcomes are presented using the custom Toast notification system.
- Sidebar collapses smoothly to maximize screen real estate for desktop users.
