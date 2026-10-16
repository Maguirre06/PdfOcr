#define AppName "PDF + OCR"
#define AppVersion "1.0.1"
#define AppPublisher "PDF + OCR"
#define AppExeName "PdfOcr.exe"
#define AppProgId "PdfOcr.pdf"

[Setup]
AppId={{E18CC50D-3E3A-4D02-A6BD-802FBF7F33B5}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={localappdata}\Programs\PdfOcr
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=..\artifacts\installer
OutputBaseFilename=PdfOcr-Setup-{#AppVersion}-win-x64
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
SetupIconFile=..\Icopdf.ico
ChangesAssociations=yes
CloseApplications=yes
SetupLogging=yes
UninstallDisplayName={#AppName}
UninstallDisplayIcon={app}\{#AppExeName}

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear un acceso directo en el escritorio"; GroupDescription: "Accesos directos adicionales:"; Flags: unchecked

[Files]
Source: "..\artifacts\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Registry]
; Registrar la aplicacion como un visor disponible para archivos PDF.
Root: HKCU; Subkey: "Software\Classes\{#AppProgId}"; ValueType: string; ValueName: ""; ValueData: "{#AppName}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\{#AppProgId}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: """{app}\Icopdf.ico"",0"
Root: HKCU; Subkey: "Software\Classes\{#AppProgId}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExeName}"" ""%1"""
Root: HKCU; Subkey: "Software\Classes\.pdf\OpenWithProgids"; ValueType: none; ValueName: "{#AppProgId}"; Flags: uninsdeletevalue

; Hacer que Windows muestre PDF + OCR en "Aplicaciones predeterminadas".
Root: HKCU; Subkey: "Software\Classes\Applications\{#AppExeName}"; ValueType: string; ValueName: "FriendlyAppName"; ValueData: "{#AppName}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\Applications\{#AppExeName}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExeName}"" ""%1"""
Root: HKCU; Subkey: "Software\Classes\Applications\{#AppExeName}\SupportedTypes"; ValueType: string; ValueName: ".pdf"; ValueData: ""
Root: HKCU; Subkey: "Software\PdfOcr\Capabilities"; ValueType: string; ValueName: "ApplicationName"; ValueData: "{#AppName}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\PdfOcr\Capabilities"; ValueType: string; ValueName: "ApplicationDescription"; ValueData: "Visor de PDF con OCR"
Root: HKCU; Subkey: "Software\PdfOcr\Capabilities\FileAssociations"; ValueType: string; ValueName: ".pdf"; ValueData: "{#AppProgId}"
Root: HKCU; Subkey: "Software\RegisteredApplications"; ValueType: string; ValueName: "PdfOcr"; ValueData: "Software\PdfOcr\Capabilities"; Flags: uninsdeletevalue

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Iniciar {#AppName}"; Flags: nowait postinstall skipifsilent
Filename: "ms-settings:defaultapps?registeredAppUser=PdfOcr"; Description: "Elegir {#AppName} como aplicacion PDF predeterminada"; Flags: shellexec postinstall skipifsilent
