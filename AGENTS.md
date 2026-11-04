# AGENTS.md

Binding rules for AI agents working in this repository. "Must" and "never" are not negotiable. If a task conflicts with a rule, stop and ask the user.

ClassPulse is a Windows desktop app that teachers use to manage classes, students, grades and notes. It uses .NET 8, Avalonia 12, CommunityToolkit.Mvvm, and EF Core 8 with SQLite.

## Read before you change

| You are changing | Read first |
|---|---|
| Anything | [docs/architecture.md](docs/architecture.md) |
| Grades, averages, participation, entities | [docs/domain.md](docs/domain.md) |
| Entities, `AppDbContext`, stored data | [docs/data.md](docs/data.md), [docs/playbooks/change-data-model.md](docs/playbooks/change-data-model.md) |
| Code style, UI text, documentation comments | [docs/conventions.md](docs/conventions.md) |
| Tests | [docs/testing.md](docs/testing.md) |
| A task covered by a playbook | [docs/playbooks/](docs/playbooks/) |
| Code that looks wrong | [docs/tech-debt.md](docs/tech-debt.md) |
| A past design choice | [docs/decisions/](docs/decisions/) |

## Definition of done

Run these commands from the repository root, in this order. Each must exit with code 0.

```powershell
dotnet restore ClassPulse.slnx
dotnet format whitespace ClassPulse.slnx --verify-no-changes --no-restore
dotnet build ClassPulse.slnx -c Release --no-restore
dotnet test ClassPulse.slnx -c Release --no-build
```

A change is done only when all of the following are true:

1. All four commands succeed, and the build reports `0 Warnung(en)/Warning(s)` and `0 Fehler/Error(s)`.
2. Every behavior change and bug fix has a test that would fail without it.
3. Every new or changed `public` or `protected` member in `ClassPulse/` has correct XML documentation.
4. All affected documents are updated (see [Keeping documentation current](#keeping-documentation-current)).
5. You report the commands you ran and their results. If you could not run one, say so.

CI runs the same checks on every pull request.

## Commands to avoid

- **Never run `dotnet format` without the `whitespace` subcommand.** It ignores the `NUnit.Analyzers` suppressions and rewrites valid test code. The build already enforces analyzers and code style.
- **Never run the app or `build.ps1` without the user's approval.** See [Data protection](#data-protection) and [docs/playbooks/release.md](docs/playbooks/release.md).

## Build rules

These rules are configured in `Directory.Build.props`, `.editorconfig` and `global.json`.

- Warnings are errors, in all projects and configurations.
- Never add `#pragma warning disable`, `[SuppressMessage]`, `NoWarn`, `WarningsNotAsErrors` or a `dotnet_diagnostic` severity to get a build passing. Fix the cause.
- Never weaken `Directory.Build.props`, `.editorconfig`, `global.json` or `.github/workflows/ci.yml` unless the user asks for it. Record any such change in an ADR.
- The only allowed exceptions are listed in [ADR 0002](docs/decisions/0002-build-enforced-rules.md).
- `Resources.Designer.cs` is generated. Change it only as described in [Add UI text](docs/playbooks/add-ui-text.md).

## Architecture rules

The dependency direction is `Views -> ViewModels -> Services -> Data`.

- `Data/` must not reference Avalonia, view models or services.
- `Services/` must not reference Avalonia, except `AvaloniaDialogService`.
- `ViewModels/` must not create windows or reference `Views`. Use `IDialogService`.
- View code-behind must contain only `InitializeComponent()`. Put logic in the view model.
- View models access data only through service interfaces, never through `AppDbContext`.
- Change entities only through their constructors and methods. Keep setters private.
- Validate domain rules in the entity, not only in the UI.

[docs/tech-debt.md](docs/tech-debt.md) lists existing violations. Never copy them into new code. Fix one only if the task asks for it.

## Code rules

- Use explicit types, never `var`.
- Use braces on every control-flow statement.
- Name private fields `_camelCase`.
- Keep `ClassPulse/` free of implicit usings. Add each `using` explicitly and remove unused ones.
- Give new `async` methods the `Async` suffix. Always `await` asynchronous calls, and never block with `.Result` or `.Wait()`.
- In `ClassPulse/`, use the null-forgiving operator `!` only in EF query expressions and in `ViewLocator`.
- Write all UI text in both `Resources.resx` and `Resources.de.resx`. Never hardcode it.
- Use only colors and brushes from `Styles/AppTheme.axaml` through `DynamicResource`, so that both themes work.
- Format numbers shown to users with `CultureInfo.CurrentCulture`.
- Keep changes within the task. Never refactor, rename or reformat unrelated code along the way.

## Dependencies

- Never add, remove or upgrade a package without the user's approval.
- Reference exact package versions.
- A vulnerability warning (`NU1901` to `NU1904`) does not fail the build. Always report it to the user.

## Data protection

The app stores personal data about children, including grades and teacher notes. The real data is on this machine.

- **Never read, copy, change or delete** `%LOCALAPPDATA%\ClassPulse\`. It contains `classpulse.db` and `settings.json`.
- The app always uses that folder, and there is no sandbox mode. Running it can change real data.
- Never commit a database, backup or export. Use only invented names in tests, examples, documents and logs.
- Never send student data to an external service.

## Git

- Create branches as `issue/<number>` from `main`.
- Write commit messages as `ISSUE <number> <type>: <imperative summary>`, where type is one of `feat`, `fix`, `docs`, `refactor`, `test`, `build` or `chore`.
- Each commit must build and pass its tests.
- Commit, push, open pull requests or merge only when the user asks.
- Never commit directly to `main`, force-push, or bypass hooks or signing.

## Keeping documentation current

Update the document in the same commit or pull request as the change.

| If you change | Update |
|---|---|
| Project structure, dependency injection, navigation, startup | `docs/architecture.md` |
| A domain rule, calculation or invariant | `docs/domain.md` |
| An entity, relationship, seed data or the schema updater | `docs/data.md` |
| A code or UI convention | `docs/conventions.md`, `AGENTS.md` |
| Test structure or test tooling | `docs/testing.md` |
| A build, CI or tooling command | `AGENTS.md`, `.github/workflows/ci.yml` |
| A significant design decision | A new ADR in `docs/decisions/` |
| A problem you find but do not fix | `docs/tech-debt.md` |
| A problem listed in tech debt that you fix | Remove its entry from `docs/tech-debt.md` |

Write documents and code comments in English. If code and documentation disagree, trust the code, fix the document, and tell the user.
