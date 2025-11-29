; FlowVision Installer Script for Inno Setup
; This script creates a single-file installer that includes all dependencies

#define MyAppName "FlowVision"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "FlowVision"
#define MyAppExeName "FlowVision.exe"
#define MyAppURL "https://github.com/flowvision"

[Setup]
; Basic installer settings
AppId={{F8E2D3A4-5B6C-7D8E-9F0A-1B2C3D4E5F6A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
; Output settings
OutputDir=installer
OutputBaseFilename=FlowVision-Setup-{#MyAppVersion}
; Compression - use LZMA2 for best compression of large files
Compression=lzma2/ultra64
SolidCompression=yes
LZMAUseSeparateProcess=yes
LZMANumBlockThreads=4
; UI settings
WizardStyle=modern
SetupIconFile=FlowVision\recursive-control-icon.ico
; Privileges
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
; Architecture
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
; Disk space info
DiskSpanning=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Main executable (contains embedded managed DLLs and detection model)
Source: "FlowVision\bin\Release\FlowVision.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "FlowVision\bin\Release\FlowVision.exe.config"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist

; Native DLLs (required - cannot be embedded in .NET exe)
Source: "FlowVision\bin\Release\onnxruntime.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "FlowVision\bin\Release\onnxruntime_providers_shared.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "FlowVision\bin\Release\tesseract50.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "FlowVision\bin\Release\leptonica-1.82.0.dll"; DestDir: "{app}"; Flags: ignoreversion

; Any additional DLLs that weren't embedded
Source: "FlowVision\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist

; Tesseract OCR data
Source: "FlowVision\bin\Release\tessdata\*"; DestDir: "{app}\tessdata"; Flags: ignoreversion recursesubdirs createallsubdirs

; Florence-2 Caption Models (large ONNX files)
Source: "FlowVision\bin\Release\models\*"; DestDir: "{app}\models"; Flags: ignoreversion recursesubdirs createallsubdirs

; Playwright browser automation files
Source: "FlowVision\bin\Release\.playwright\*"; DestDir: "{app}\.playwright"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist

; Native x64/x86 libraries
Source: "FlowVision\bin\Release\x64\*"; DestDir: "{app}\x64"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist
Source: "FlowVision\bin\Release\x86\*"; DestDir: "{app}\x86"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist

; Web UI files (HTML, CSS, JS)
Source: "FlowVision\bin\Release\*.html"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "FlowVision\bin\Release\*.css"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "FlowVision\bin\Release\*.js"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
function InitializeSetup(): Boolean;
begin
  Result := True;
end;

