# ADR 0005: Check changes locally with Git hooks

- **Status:** Accepted
- **Date:** 2026-11-05

## Context

CI checked formatting, analyzers and tests, but only after a push. Nothing checked commit messages or secrets, and ADR 0002 left the data protection rules to `AGENTS.md` and review. The repository is public, and the app stores personal data about children, so a committed database could not be fully taken back.

## Decision

Use the [pre-commit](https://pre-commit.com/) framework, configured in `.pre-commit-config.yaml`:

| Stage | Checks |
|---|---|
| `pre-commit` | Merge conflict markers, large files, secrets (gitleaks), data files, `dotnet format whitespace` on changed C# files |
| `commit-msg` | The commit message format from `AGENTS.md` |
| `pre-push` | Release build and unit tests |

The build runs at `pre-push` because linting needs a full build, which is too slow for every commit.

The framework needs Python. Husky.NET would not, but the framework also provides the secret and conflict checks without extra scripts. Our own checks are PowerShell scripts in `scripts/hooks/`, because PowerShell is on every supported machine and the tests can run them.

CI stays the authority, because hooks must be installed first and can be skipped. It runs the same data file check on all tracked files, and it adds:

- Dependabot for NuGet and GitHub Actions. It only opens pull requests, so every update still needs approval.
- CodeQL for C#, in `build-mode: none`, so it needs no Windows runner.
- A line coverage figure in the job summary, without a required minimum.

The data file check compares path names only and never opens a file. The app has no export feature today, so `.csv`, `.xls` and `.xlsx` are blocked as a precaution.

## Consequences

- A push takes as long as a build and a test run.
- A commit that changes C# files takes as long as `dotnet format` needs to load the solution.
- `.csv` and Excel files cannot be committed. Allowing one needs a change to `scripts/hooks/Test-DataFiles.ps1`.
- Commits made on GitHub, such as those from Dependabot, do not pass through the hooks and need not follow the commit message format.
- `dotnet test` in the `pre-push` hook can create `%LOCALAPPDATA%\ClassPulse\` through `UtilsTests`, as described in [testing.md](../testing.md). No hook reads files in that folder.
- GitHub must still require the CI check on `main` for it to block merges, as ADR 0002 says. This is a repository setting.
