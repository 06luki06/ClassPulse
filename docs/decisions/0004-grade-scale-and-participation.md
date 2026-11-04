# ADR 0004: Grade scale and non-participation

- **Status:** Accepted
- **Date:** 2026-11-04, recorded afterward from the code

## Context

A student can miss an assessment. That result must stay visible, but must not affect statistics.

## Decision

- Grades run from 1.0 (best) to 5.0 (worst).
- `Assessment.HasParticipated` records participation. Older rows default to `true`.
- A grade of 0.0 always means non-participation, and it forces `HasParticipated` to `false`.
- Statistics count only results with `HasParticipated == true` and `Grade > 0.0`.

## Consequences

- Any code that reads `Grade` must handle 0.0. Treat it as "no grade", never as a real value.
- A non-participating result can still hold a grade from 1.0 to 5.0. It is stored but not counted.
- The scale is fixed in code. Supporting another grading system would require a new decision.
