# Change the data model

First, check whether the change reaches existing databases, using the table in [data.md](../data.md#what-reaches-existing-databases).

## New scalar property

1. Add the property to the entity with a private setter and XML documentation.
2. Add it to the relevant constructor and update method, with validation where needed.
3. If existing rows need a specific value, configure `HasDefaultValue` in `AppDbContext`.
4. For a `bool` column with the default `true`, also configure `ValueGeneratedNever`. See [data.md](../data.md#hasparticipated).
5. Add entity tests, and add a test to `Tests/Data/DatabaseSchemaUpdaterTests.cs` if the column has a default.
6. Update [data.md](../data.md), and [domain.md](../domain.md) if a rule changes.
7. Run the checks in the [definition of done](../../AGENTS.md#definition-of-done).

## Any other change

This covers new tables, renames, removals, type or nullability changes, constraints, indexes and seed data.

**Stop and ask the user.** `DatabaseSchemaUpdater` cannot deliver these changes to existing databases, and installed copies hold real data. The change needs a new ADR and a migration plan that has been tested against a database in the old format.

## Never

- Delete or recreate a database to get past a schema problem.
- Test against the real database in `%LOCALAPPDATA%\ClassPulse\`.
