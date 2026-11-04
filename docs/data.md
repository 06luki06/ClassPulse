# Data

## Storage

| File | Contents | Path |
|---|---|---|
| `classpulse.db` | SQLite database | `%LOCALAPPDATA%\ClassPulse\` |
| `settings.json` | Theme and language | `%LOCALAPPDATA%\ClassPulse\` |

These paths are fixed in code, and they hold real personal data. See [Data protection](../AGENTS.md#data-protection).

## Model

```text
SchoolClass 1 --- n Student        cascade delete
Student     1 --- n Assessment     cascade delete
Student     1 --- n SubjectNote    cascade delete
Subject     1 --- n Assessment     restrict delete
Subject     1 --- n SubjectNote    restrict delete
```

All primary keys are GUIDs created in the entity constructor.

## Entity pattern

- Properties have private setters.
- Each entity has a private parameterless constructor for EF Core.
- Public constructors and methods validate their input.
- Collections are private `List<T>` fields, exposed as `IReadOnlyCollection<T>`.

## Automatic loading

`AppDbContext` configures these navigations with `AutoInclude`, so they load with every query:

- `Student`: `SchoolClass`, `Assessments`, `SubjectNotes`
- `Assessment`: `Subject`
- `SubjectNote`: `Subject`

`SchoolClass.Students` does not load automatically, so `GetAllClassesAsync` includes it explicitly.

## Seed data

`OnModelCreating` seeds three subjects with fixed IDs: Mathematik (`M`), Deutsch (`D`) and Sachunterricht (`SU`).

Seed data is written only when `EnsureCreated` creates a new database. **Changing it does not affect existing databases.**

## Schema updates

The app does not use EF Core migrations, although the tools package is referenced. See [ADR 0003](decisions/0003-additive-schema-updates.md).

At each start, `DatabaseSchemaUpdater.EnsureCreatedAndUpToDate` does the following:

1. Calls `EnsureCreated`, which only creates a database that does not exist yet.
2. Compares every mapped column with `PRAGMA table_info`, and runs `ALTER TABLE ... ADD COLUMN` for each missing one.

### What reaches existing databases

| Model change | Reaches existing databases |
|---|---|
| New scalar property or column | Yes |
| New table or entity | **No** |
| Renamed or removed column | **No** |
| Changed type, nullability or default of an existing column | **No** |
| New index, key or foreign key constraint | **No** |
| New or changed seed data | **No** |

A change marked "No" must not be made without a migration plan agreed with the user. Otherwise, teachers' installed copies may fail or lose data on update.

### Defaults for new columns

The default of a new column, in order of priority:

1. The configured `HasDefaultValueSql`
2. The configured `HasDefaultValue`
3. For non-nullable columns, the CLR default (`0`, `false`, or an empty string)

Existing rows receive this value.

### `HasParticipated`

`Assessment.HasParticipated` has the database default `true`, because all older results count as participated. It is also configured with `ValueGeneratedNever`. Without this, EF Core would treat `false` as "not set" and store the database default `true`. Use this pair for any new `bool` column whose default is `true`.

## Concurrency and tracking

All services share one `AppDbContext` for the app's lifetime (TD-01). Loaded entities stay tracked, so queries can return already tracked instances, and overlapping operations on the context are not safe.
