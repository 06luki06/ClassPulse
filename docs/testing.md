# Testing

Run the tests with:

```powershell
dotnet test ClassPulse.slnx -c Release
```

## Structure

`Tests/` mirrors `ClassPulse/`:

| Path | Tests | Dependencies |
|---|---|---|
| `Tests/Data/Entities/` | Validation and behavior of entities | None |
| `Tests/Data/DatabaseSchemaUpdaterTests.cs` | Schema updates and database defaults | In-memory SQLite |
| `Tests/Data/DatabaseSchemaUpdaterOldFormatTests.cs` | A database in the format of the oldest installed copies still opens and updates. Its schema and values are copied from a database created with EF Core 8, so do not regenerate them | SQLite in a temporary file |
| `Tests/Services/` | Services | EF Core InMemory, temporary files for settings |
| `Tests/ViewModels/` | View model logic | Stubs from `Tests/Stubs/` |
| `Tests/ResourcesTests.cs` | Matching keys and non-blank values in both languages | Compiled resources |
| `Tests/ReleaseScriptsTests.cs` | `build.ps1` and `ClassPulseSetup.iss` take the version only from `ClassPulse.csproj`, and the installer packages the output folder of its target framework. Reads the script text and runs neither script | Files in the repository root |
| `Tests/HookScriptsTests.cs` | The scripts in `scripts/hooks/` block data files and invalid commit messages, and the hook and CI configuration use them correctly | Runs the scripts with `powershell`, temporary files |
| `Tests/UtilsTests.cs` | Helper methods | File system. Creates `%LOCALAPPDATA%\ClassPulse\` if it is missing, and reads no files in it |

The project has no UI tests. Do not rely on tests to check AXAML layout or bindings.

## Rules

- Use NUnit with `Assert.That`.
- Name tests `Method_Condition_ExpectedResult`.
- Each test must set up its own state, with a unique database name and temporary file paths.
- Tests must not access `classpulse.db` or `settings.json` in `%LOCALAPPDATA%\ClassPulse\`. `UtilsTests` is the only test allowed to use that folder.
- Use invented names such as "Max Mustermann".
- Do not weaken or delete a test to get a build passing. If a behavior changes on purpose, change the test in the same commit and say why.

## Choosing a test database

| Purpose | Use |
|---|---|
| Service logic and queries | EF Core InMemory, `UseInMemoryDatabase(Guid.NewGuid().ToString())` |
| SQL, defaults, schema and constraints | SQLite with `Data Source=:memory:` and an open connection |

EF Core InMemory does not behave like SQLite. It ignores relational constraints and does not run SQL, so any test of database behavior must use SQLite.

## Stubs

`Tests/Stubs/` has `ClassServiceStub`, `AssessmentServiceStub` and `DialogServiceStub`. When an interface changes, update its stub too.

## Test patterns

Testing that invalid input is rejected:

```csharp
[TestCase(null)]
[TestCase("")]
[TestCase("   ")]
public void Constructor_WithInvalidName_ThrowsArgumentNullException(string? invalidName)
{
    void action()
    {
        _ = new Subject(invalidName!, "M");
    }

    Assert.Throws<ArgumentNullException>((Action)action);
}
```

Use `!` when a test passes `null` on purpose, and `_ =` when it creates an object only to trigger an exception.

## Limitations

- `App.Current` is `null` in tests, so navigation through `App.Current.Services` does nothing there. Assert on state, not on navigation.
- View models that open windows directly cannot be tested fully (TD-02).
- `App`, `Program`, `ViewLocator` and the views have `[ExcludeFromCodeCoverage]`.

## Coverage

`coverlet.collector` is installed. There is no required coverage threshold. CI shows the line coverage in the job summary.

```powershell
dotnet test ClassPulse.slnx --collect:"XPlat Code Coverage"
```
