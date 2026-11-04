# Technical debt

These are known problems in the existing code. Never copy these patterns into new code. Fix an item only if your task asks for it, and then remove its entry.

All items come from reading the code on 2026-11-04. They have not been reproduced in the running app.

## Possible defects

| ID | Location | Problem | Effect |
|---|---|---|---|
| TD-04 | `AssessmentGradeEntryDto`, `Assessment.ValidateGrade` | The grade editor allows 0.5 while participation stays on, but the entity rejects this value. | Saving throws `ArgumentOutOfRangeException` and the change is lost. |
| TD-05 | `ClassDetailViewModel.AddAssessmentAsync` | New assessments give every student a default grade of 1.0. | Averages look better until a teacher edits every result. |
| TD-06 | `ClassDetailViewModel.AddAssessmentAsync` | The weight is parsed as `int`, although `Assessment.Weight` is `double`. | Input such as `0.5` closes the dialog without saving or a message. |
| TD-07 | `App.ShowErrorDialog` | The error dialog does not subscribe to `OnCloseRequested`. | Its buttons do not close it. Only the window's close button does. |
| TD-08 | `HomeViewModel.DeleteClassAsync`, `ClassDetailViewModel.RemoveStudent` | Deleting has no confirmation, although it permanently removes grades and notes. | One click can lose data. Commit `5743d3e` names this behavior, so check with the user before changing it. |
| TD-09 | `HomeViewModel.CreateClassAsync` and others | Input is not validated before it reaches the entity. | An invalid school year appears as a general error dialog. |
| TD-10 | `SettingsService` | Empty `catch` blocks hide read and write failures. | Settings can be lost without any message. |

## Architecture

| ID | Location | Problem | Effect |
|---|---|---|---|
| TD-01 | `App.ConfigureServices` | Scoped services are resolved from the root provider, so one `AppDbContext` lives for the whole app. | Tracked entities can be stale, and overlapping operations can throw. Scope validation is off, so nothing reports this. |
| TD-02 | View models | Navigation uses `App.Current.Services`, and `StudentDetailViewModel` creates `AssessmentGradeEditWindow` itself. | Breaks layering and limits testing. |
| TD-03 | View model constructors | Loading starts as a discarded task with `_ = LoadDataAsync()`. | Errors can appear late or not at all, and a load can overlap other operations. |
| TD-11 | `App.axaml.cs`, `ClassDetailViewModel`, `ViewLocator` | Some UI text is hardcoded: error dialog titles, `Max`, `Mustermann`, `M`, `Not Found`. | This text is not translated. |
| TD-12 | `AppDbContext` | Seeded subject names are German only. | English users see German names. |

## Model consistency

| ID | Location | Problem |
|---|---|---|
| TD-13 | `Student.FirstName`, `Student.LastName`, `SubjectNote.Text` | These are declared nullable, although the constructors reject blank values. |
| TD-14 | Entities | Blank strings and empty IDs throw different exception types across entities. See [domain.md](domain.md#validation). |
| TD-15 | `Assessment.Title` | Unlike other text, the title is not trimmed. |
| TD-16 | `SubjectNote` constructor | It reads `DateTime.Now` directly, so tests cannot control the time. |

## Unused code

Nothing in the app calls these members. Check with the user before removing them, because they may be for planned features.

- `IClassService.SearchStudentAsync`
- `IAssessmentService.GetAssessmentsByTitleAndSubjectAsync`
- `IAssessmentService.AddSubjectNoteAsync`, which means the UI cannot create subject notes
- `Student.UpdateDetails`, `SubjectNote.UpdateText`

## Build and platform

| ID | Location | Problem |
|---|---|---|
| TD-17 | Target framework | Support for .NET 8 and EF Core 8 ends on 2026-11-10. Upgrading requires changes to both project files, `ClassPulseSetup.iss` and the CI runtime. |
| TD-18 | `ClassPulse.csproj` | `AvaloniaUI.DiagnosticsSupport` and `Avalonia.Markup.Xaml.Loader` are copied to Release output. A comment describes a condition that does not exist. HotAvalonia's hot reload is enabled only in Debug by default. |
| TD-19 | `ClassPulse.csproj`, `ClassPulseSetup.iss` | The version is stored in both files and must be changed manually. |
