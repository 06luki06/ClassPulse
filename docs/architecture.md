# Architecture

ClassPulse is a single-process MVVM desktop app. It has no server, and all data is in a local SQLite file.

## Technology

| Purpose | Technology |
|---|---|
| Runtime | .NET 8, C# 14, built with the SDK from `global.json` |
| UI | Avalonia 12 with the Fluent theme, FluentIcons, compiled bindings |
| MVVM | CommunityToolkit.Mvvm source generators |
| Data | EF Core 8 with SQLite |
| Dependency injection | Microsoft.Extensions.DependencyInjection |
| Tests | NUnit 4, EF Core InMemory, in-memory SQLite |
| Installer | Inno Setup |

## Structure

| Path | Responsibility |
|---|---|
| `ClassPulse/Program.cs` | Process entry point and Avalonia configuration |
| `ClassPulse/App.axaml(.cs)` | Startup, dependency injection, global exception handlers, styles |
| `ClassPulse/ViewLocator.cs` | Maps view models to views by naming convention |
| `ClassPulse/Utils.cs` | Locates the application data folder |
| `ClassPulse/Data/` | `AppDbContext`, `AppSettings`, `DatabaseSchemaUpdater` |
| `ClassPulse/Data/Entities/` | Domain entities and their validation |
| `ClassPulse/Services/` | Data access, calculations, settings and dialogs |
| `ClassPulse/ViewModels/` | Page state and commands |
| `ClassPulse/ViewModels/Dialogs/` | Dialog state, including `InputField` |
| `ClassPulse/Views/` | AXAML pages and dialog windows |
| `ClassPulse/Resources/` | Localized text |
| `ClassPulse/Styles/AppTheme.axaml` | Colors and brushes for light and dark themes |
| `Tests/` | Automated tests, mirroring the `ClassPulse/` folders |
| `build.ps1`, `ClassPulseSetup.iss` | Publishing and installer |
| `.pre-commit-config.yaml`, `scripts/hooks/` | Git hooks. See [ADR 0005](decisions/0005-git-hooks.md) |
| `.github/` | CI, CodeQL and Dependabot |

## Layers

```text
Views (AXAML)
   | compiled bindings
ViewModels
   | service interfaces
Services
   | AppDbContext
Data and Entities
   |
SQLite: %LOCALAPPDATA%\ClassPulse\classpulse.db
```

## Startup

`App.OnFrameworkInitializationCompleted` performs these steps in order:

1. Registers the handlers for unhandled exceptions.
2. Builds the service provider.
3. Loads the settings and applies the theme and culture.
4. Calls `DatabaseSchemaUpdater.EnsureCreatedAndUpToDate` in a short-lived scope.
5. Resolves `MainWindowViewModel` and opens `MainWindow`.

## Dependency injection

Registrations are in `App.ConfigureServices`.

| Service | Registered lifetime |
|---|---|
| `AppDbContext` | Scoped |
| `IClassService` / `ClassService` | Scoped |
| `IAssessmentService` / `AssessmentService` | Scoped |
| `IDialogService` / `AvaloniaDialogService` | Scoped |
| `SettingsService` | Singleton |
| `HomeViewModel`, `SettingsViewModel` | Transient |
| `MainWindowViewModel` | Singleton |

**The registered lifetimes do not match the effective ones.** All view models are resolved from the root provider, so the scoped services and `AppDbContext` act as one instance for the whole app lifetime. Only the startup schema update uses its own scope. See TD-01 in [tech-debt.md](tech-debt.md).

`ClassDetailViewModel` and `StudentDetailViewModel` are not registered. They need runtime arguments, so the calling view model creates them and passes its services on.

## Navigation

`MainWindow` contains a `ContentControl` bound to `MainWindowViewModel.CurrentView`. Setting that property changes the page, and `ViewLocator` creates the matching view.

| View model | View |
|---|---|
| `HomeViewModel` | `HomeView` |
| `ClassDetailViewModel` | `ClassDetailView` |
| `StudentDetailViewModel` | `StudentDetailView` |
| `SettingsViewModel` | `SettingsView` |

`ViewLocator` replaces `ViewModel` with `View` in the full type name. A page therefore works only if the namespace and class name follow this pattern. It handles only types that derive from `ViewModelBase`.

View models navigate through `App.Current.Services`. See TD-02 in [tech-debt.md](tech-debt.md).

## Dialogs

| Dialog | View model | Opened by |
|---|---|---|
| `InputDialogWindow` | `InputDialogViewModel` | `IDialogService.ShowInputDialogAsync` |
| `MessageDialogWindow` | `MessageDialogViewModel` | `IDialogService.ShowMessageAsync`, for example to report invalid input |
| `AssessmentGradeEditWindow` | `AssessmentGradeEditViewModel` | `StudentDetailViewModel` directly (TD-02) |

A dialog's view model raises `OnCloseRequested`, and whoever opens the window subscribes to close it. Without that subscription, the dialog's buttons do not close it.

`InputField` supports three kinds of input: a dropdown when it has options, a multi-line box when `isMultiline` is set, and a single-line box otherwise. `InputDialogViewModel.GetValue` finds a field by its label, so labels must be unique within a dialog.

## Settings, theme and language

- `SettingsService` stores `AppSettings` in `%LOCALAPPDATA%\ClassPulse\settings.json`. If reading fails it uses the defaults, and it ignores write failures.
- `SettingsViewModel` applies and saves changes immediately.
- Supported languages are `de` (default) and `en`. The neutral resources are English.
- AXAML reads text through `x:Static`, which is evaluated when a view is created. Text changes after a language switch only in views created afterward.

## Error handling

`App` handles three kinds of unhandled exceptions: UI thread, unobserved task and app domain. It shows the exception message in an `InputDialogWindow`. Handled UI thread exceptions do not end the app.

View model constructors start loading with `_ = LoadDataAsync()`. An exception in such a discarded task reaches the handler for unobserved tasks only when the task is finalized, so it can appear late or not at all. See TD-03 in [tech-debt.md](tech-debt.md).
