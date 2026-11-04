# Release a version

Only do this when the user asks for it.

## Requirements

- Windows
- The SDK from `global.json`
- Inno Setup, with `iscc.exe` on the `PATH`

## Steps

1. Check that the [definition of done](../../AGENTS.md#definition-of-done) passes on the release commit.
2. Set the same version in both places:
   - `Version` in `ClassPulse/ClassPulse.csproj`
   - `MyAppVersion` in `ClassPulseSetup.iss`
3. Run `.\build.ps1` from the repository root. It does the following:
   - publishes a self-contained single-file `win-x64` build
   - runs `iscc.exe ClassPulseSetup.iss`
4. The installer is created at `InstallerOutput\ClassPulse_Setup_<version>.exe`. This folder is ignored by Git.

## Notes

- Never change `AppId` in `ClassPulseSetup.iss`. Installers use it to recognize and update an existing installation.
- `SourcePath` in `ClassPulseSetup.iss` contains the target framework. Update it if the target framework changes.
- Updating the app runs `DatabaseSchemaUpdater` against the user's real database at next start. Confirm that any model change is covered by [data.md](../data.md#what-reaches-existing-databases) before release.
