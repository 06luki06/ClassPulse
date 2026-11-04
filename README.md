# ClassPulse

ClassPulse is a Windows desktop app that helps teachers manage classes, students, grades and notes. All data stays on the local computer.

## Development

You need Windows and the .NET SDK version from `global.json`, plus the .NET 8 runtime.

```powershell
dotnet build ClassPulse.slnx
dotnet test ClassPulse.slnx
dotnet run --project ClassPulse
```

Running the app uses your real data in `%LOCALAPPDATA%\ClassPulse\`.

## Documentation

- [AGENTS.md](AGENTS.md): binding rules for AI agents and the definition of done
- [docs/](docs/README.md): architecture, domain, data, conventions, testing, decisions and playbooks
