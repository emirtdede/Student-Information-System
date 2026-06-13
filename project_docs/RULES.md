# Project Rules & Guidelines

## Prohibitions (Yasaklar)
- **NO** file deletion without explicit user confirmation.
- **NO** rewriting of already working/tested layouts or components unless specifically asked.
- **NO** switching to another stack (e.g., using pure CSS variables and Tailwind class patterns instead of forcing SCSS/Bootstrap).
- **NO** concurrent modification of unrelated controllers or modules.
- **NO** leaving running `dotnet run` instances in background tasks at the end of the session.

## Requirements (Zorunluluklar)
- Always split large tasks into clean, sequential commits/steps.
- Maintain localization support: all UI text must have `.lang-tr` and `.lang-en` spans, and backend messages must pass through `LocalizationHelper`.
- Update `project_docs/MEMORY.md` (keep it short), `project_docs/TASKS.md`, and `project_docs/CHANGELOG.md` at the end of every task execution.
- Maintain CSS/HTML accessibility and clean contrast rules.
