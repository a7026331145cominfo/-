#define MyAppName "AlSaqarAccounting"
#define MyAppVersion "1.0.5"
#define MyAppPublisher "AlSaqarAccounting"
#define MyAppExeName "AlSaqarAccounting.exe"

[Setup]
AppId={{B8D7D5A1-8D1A-4A6D-9E8B-1001A1A1001}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\AlSaqarAccounting
DefaultGroupName={#MyAppName}
OutputDir="C:\Users\hp\Downloads\AlSaqarAccountingV4-phase1-final\AlSaqarAccountingV4-main\Output"
OutputBaseFilename=AlSaqarAccounting-Setup-v1.0.5
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
WizardStyle=modern
UninstallDisplayName={#MyAppName} {#MyAppVersion}

[Files]
Source: "C:\Users\hp\Downloads\AlSaqarAccountingV4-phase1-final\AlSaqarAccountingV4-main\publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{autoprograms}\AlSaqarAccounting"; Filename: "{app}\AlSaqarAccounting.exe"
Name: "{autodesktop}\AlSaqarAccounting"; Filename: "{app}\AlSaqarAccounting.exe"

[Run]
Filename: "{app}\AlSaqarAccounting.exe"; Description: "Run AlSaqar Accounting"; Flags: nowait postinstall skipifsilent