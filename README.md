# ClassPulse

ClassPulse is a Windows desktop app that helps teachers manage classes, students, grades and notes. All data stays on the local computer.

## Development

You need Windows and the .NET SDK version from `global.json`, which includes the .NET 10 runtime.

```powershell
dotnet build ClassPulse.slnx
dotnet test ClassPulse.slnx
dotnet run --project ClassPulse
```

Running the app uses your real data in `%LOCALAPPDATA%\ClassPulse\`.

### Git hooks

The hooks need Python. Install them once per clone, in a virtual environment:

```powershell
python -m venv .venv
.venv\Scripts\python -m pip install pre-commit==4.7.0
.venv\Scripts\pre-commit install
```

They check every commit and push from then on, without activating the environment. If you delete or move `.venv`, repeat these steps. To run the commit checks on all files:

```powershell
.venv\Scripts\pre-commit run --all-files
```

## Documentation

- [AGENTS.md](AGENTS.md): binding rules for AI agents and the definition of done
- [docs/](docs/README.md): architecture, domain, data, conventions, testing, decisions and playbooks
