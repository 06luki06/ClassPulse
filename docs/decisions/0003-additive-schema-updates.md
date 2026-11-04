# ADR 0003: Additive schema updates instead of migrations

- **Status:** Accepted
- **Date:** 2026-11-04, recorded afterward from the code

## Context

The app creates its database with `EnsureCreated`, which does nothing to a database that already exists. Columns added to entities later were therefore missing from installed copies. The reasoning for not using EF Core migrations was not recorded.

## Decision

`DatabaseSchemaUpdater` runs at every start and adds each mapped column that is missing. It makes additive column changes only.

## Consequences

- Adding a scalar property needs no extra update code.
- New tables, renames, removals, type changes, constraints and seed changes do not reach existing databases. See [data.md](../data.md#what-reaches-existing-databases).
- Any such change needs a new decision, because this approach cannot deliver it. Introducing EF Core migrations would be one option, and would have to handle databases that already exist without a migration history.
