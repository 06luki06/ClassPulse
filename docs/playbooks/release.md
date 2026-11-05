# Release a version

Only do this when the user asks for it.

## Requirements

- Windows
- The SDK from `global.json`
- Inno Setup, with `iscc.exe` on the `PATH`

## Steps

1. Check that the [definition of done](../../AGENTS.md#definition-of-done) passes on the release commit.
2. Set the version in `Version` in `ClassPulse/ClassPulse.csproj`. This is the only place that defines it.
3. Run `.\build.ps1` from the repository root. It does the following:
   - reads the version with `dotnet msbuild ClassPulse\ClassPulse.csproj -getProperty:Version`
   - publishes a self-contained single-file `win-x64` build
   - runs `iscc.exe "/DMyAppVersion=<version>" ClassPulseSetup.iss`
4. The installer is created at `InstallerOutput\ClassPulse_Setup_<version>.exe`. This folder is ignored by Git.

## Notes

- Never change `AppId` in `ClassPulseSetup.iss`. Installers use it to recognize and update an existing installation.
- `ClassPulseSetup.iss` defines no version. Compiling it without `/DMyAppVersion=<version>` stops with an error, so use `build.ps1`.
- `SourcePath` in `ClassPulseSetup.iss` contains the target framework. Update it if the target framework changes.
- Updating the app runs `DatabaseSchemaUpdater` against the user's real database at next start. Confirm that any model change is covered by [data.md](../data.md#what-reaches-existing-databases) before release.
