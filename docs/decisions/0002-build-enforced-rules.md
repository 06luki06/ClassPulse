# ADR 0002: Enforce project rules in the build

- **Status:** Accepted
- **Date:** 2026-11-04

## Context

The repository is being set up for AI agents. A rule that exists only in a document depends on the agent following it. The build previously reported 18 warnings, and nothing checked formatting, documentation or the SDK version.

## Decision

Rules are enforced by tools wherever possible:

- `TreatWarningsAsErrors` in all projects
- `AnalysisLevel=latest-recommended` and `EnforceCodeStyleInBuild`
- XML documentation for all `public` and `protected` members of `ClassPulse/`, through `CS1591`
- A pinned SDK in `global.json`
- `dotnet format whitespace`, build and tests in CI on every pull request

### Allowed exceptions

This is the complete list. Adding to it requires a change to this ADR and the user's approval.

| Rule | Scope | Configured in | Reason |
|---|---|---|---|
| `CS1591` | `Tests/` | `.editorconfig` | Test classes are not an API. `GenerateDocumentationFile` stays on so `IDE0005` works. |
| `CA1707` | `Tests/` | `.editorconfig` | Test names use the format `Method_Condition_ExpectedResult`. |
| `NU1900` to `NU1904` stay warnings | All projects | `Directory.Build.props` | Advisories change over time. They should not make an unchanged commit fail. CI reports them, and agents must tell the user about them. |

### Formatting

CI uses `dotnet format whitespace`, not plain `dotnet format`. Plain `dotnet format` ignores the suppressions from `NUnit.Analyzers`, adds `required` to fields set in `[SetUp]`, and reports errors the compiler does not report. The build already covers analyzers and code style.

### Line endings

`.gitattributes` stores text as LF. `.editorconfig` does not set `end_of_line`, so Windows working copies can use CRLF without failing the format check.

## Consequences

- New warnings introduced by an SDK or package update fail the build until they are fixed.
- Most rule violations are found before review.
- Rules that tools cannot check, such as the data protection rules, still depend on `AGENTS.md` and review.
- GitHub must require the CI check on `main` for it to block merges. This is a repository setting and not part of the code.
