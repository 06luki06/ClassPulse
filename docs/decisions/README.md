# Architecture decision records

An ADR records a significant decision and its consequences. Agents must follow accepted ADRs. To change a decision, add a new ADR that replaces the old one, and ask the user first.

| ADR | Decision | Status |
|---|---|---|
| [0001](0001-layered-mvvm-desktop-app.md) | Layered MVVM desktop app with Avalonia | Accepted |
| [0002](0002-build-enforced-rules.md) | Enforce project rules in the build | Accepted |
| [0003](0003-additive-schema-updates.md) | Additive schema updates instead of migrations | Accepted |
| [0004](0004-grade-scale-and-participation.md) | Grade scale and non-participation | Accepted |

ADRs 0001, 0003 and 0004 were written afterward from the code. Their original reasoning was not recorded, so any inferred reasons are marked as such.

## Template

File name: `NNNN-short-title.md`

```markdown
# ADR NNNN: Title

- **Status:** Proposed, Accepted, or Replaced by ADR NNNN
- **Date:** YYYY-MM-DD

## Context

## Decision

## Consequences
```
