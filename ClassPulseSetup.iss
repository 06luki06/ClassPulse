; =====================================================================
; INNO SETUP SCRIPT FOR CLASSPULSE
; =====================================================================

#define MyAppName "ClassPulse"
#define MyAppPublisher "06luki06"
#define MyAppExeName "ClassPulse.exe"

#define SourcePath ".\ClassPulse\bin\Release\net10.0\win-x64\publish"

; The version is defined only in ClassPulse\ClassPulse.csproj. build.ps1 passes it in.
#ifndef MyAppVersion
  #error MyAppVersion is missing. Run build.ps1, or pass /DMyAppVersion=<version> to iscc.exe.
#endif

; build.ps1 passes the short hash of the commit that is being built.
#ifndef MyAppCommit
  #error MyAppCommit is missing. Run build.ps1, or pass /DMyAppCommit=<short hash> to iscc.exe.
#endif

#define MyAppFullVersion MyAppVersion + "+" + MyAppCommit

[Setup]
AppId={{D821F242-9921-4EBA-8A11-F99201A8325C}
AppName={#MyAppName}
AppVersion={#MyAppFullVersion}
AppPublisher={#MyAppPublisher}

UsePreviousAppDir=yes
CloseApplications=yes
RestartApplications=no

DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes

OutputDir=.\InstallerOutput
OutputBaseFilename=ClassPulse_Setup_{#MyAppFullVersion}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern

PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Main-Application
Source: "{#SourcePath}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
; More files that got created during publishing
Source: "{#SourcePath}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "{#MyAppExeName}"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent