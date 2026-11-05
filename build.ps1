$version = dotnet msbuild ClassPulse\ClassPulse.csproj -getProperty:Version
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($version)) {
    throw "Could not read Version from ClassPulse\ClassPulse.csproj."
}

$commit = git rev-parse --short HEAD
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($commit)) {
    throw "Could not read the commit hash."
}

dotnet publish ClassPulse\ClassPulse.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:UseAppHost=true

iscc.exe "/DMyAppVersion=$version" "/DMyAppCommit=$commit" ClassPulseSetup.iss
