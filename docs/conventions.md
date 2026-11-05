# Conventions

[AGENTS.md](../AGENTS.md) holds the binding rules. This document explains how to apply them.

## Enforcement

| Rule | Enforced by |
|---|---|
| No warnings | `TreatWarningsAsErrors` in `Directory.Build.props` |
| Analyzer findings | `AnalysisLevel=latest-recommended` |
| `var`, braces, unused usings, `_camelCase` | `.editorconfig` with `EnforceCodeStyleInBuild` |
| Public API documentation | `GenerateDocumentationFile` and `CS1591` |
| Formatting, encoding, final newline | `dotnet format whitespace` in CI |
| SDK version | `global.json` |
| Matching resource keys | `Tests/ResourcesTests.cs` |
| No databases, backups or exports in the repository | `scripts/hooks/Test-DataFiles.ps1` in the Git hooks and CI |
| Commit message format | `scripts/hooks/Test-CommitMessage.ps1` in the Git hooks |
| No secrets in commits | gitleaks in the Git hooks |

All other rules in `AGENTS.md` depend on careful work and review.

## C# style

- Files in `ClassPulse/` declare all usings explicitly. `Tests/` has implicit usings enabled.
- Prefer target-typed `new` and collection expressions (`[]`) where the type is visible.
- Existing code uses both block-scoped and file-scoped namespaces. Match the file you are editing, and use block-scoped namespaces in new files.
- Store text as UTF-8 without a byte order mark. Git stores line endings as LF.

## Documentation comments

Every `public` or `protected` type and member in `ClassPulse/` needs an XML comment. `Tests/` is exempt.

| Element | Requirement |
|---|---|
| Interface member | Full contract: `<summary>`, every `<param>`, `<returns>`, and `<exception>` where relevant |
| Implementation of an interface member | `/// <inheritdoc/>`, plus more only if it adds behavior |
| Override | `/// <inheritdoc/>`, or a `<summary>` if the override's behavior is specific |
| Constructor | `<summary>` and every `<param>` |
| Positional record | `<summary>` and a `<param>` for every parameter |
| Property | `<summary>` starting with "Gets" or "Gets or sets" |
| `[ObservableProperty]` field | `<summary>` on the private field. The generated property inherits it |

Describe what a caller cannot see from the signature: valid ranges, units, side effects, exceptions, and what `null` means. Do not restate the member's name. Comments must be in English.

## MVVM

- Use `[ObservableProperty]` on private `_camelCase` fields, and `[RelayCommand]` on methods.
- Use `partial void On<Property>Changed` for reactions to property changes.
- Page view models derive from `ViewModelBase`, and `ViewLocator` maps them to views.
- Set `x:DataType` in AXAML. Compiled bindings are on by default.

## UI text

All user-visible text belongs in `ClassPulse/Resources/`:

- `Resources.resx`: English, used as the neutral language
- `Resources.de.resx`: German

Keys are prefixed by purpose: `Label_` for shared labels, `Dialog_<Name>_` for dialogs, `General_` for general messages, and the view name for text used by one view, such as `Home_` or `StudentDetailView_`.

Use `{x:Static res:Resources.Key}` in AXAML, and `Resources.Resources.Key` in C#. See [Add UI text](playbooks/add-ui-text.md).

## Styling

Define colors and brushes in `Styles/AppTheme.axaml` for both themes, and use them through `{DynamicResource KeyName}`. Styles for the whole app are in `App.axaml`.

The app uses FluentIcons for icons.
