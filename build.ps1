dotnet publish ClassPulse\ClassPulse.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:UseAppHost=true

iscc.exe ClassPulseSetup.iss