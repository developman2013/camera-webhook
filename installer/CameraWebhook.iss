#define MyAppName "Camera Webhook"
#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif
#ifndef PublishDir
  #define PublishDir "..\artifacts\publish"
#endif

[Setup]
AppId={{4BC8D4D6-584F-48BD-930C-8C0B5DD1D158}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=developman2013
AppPublisherURL=https://github.com/developman2013/camera-webhook
DefaultDirName={localappdata}\Programs\CameraWebhook
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=..\artifacts\installer
OutputBaseFilename=CameraWebhook-Setup-{#MyAppVersion}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\CameraWebhook.exe
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
CloseApplications=yes

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Camera Webhook"; Filename: "{app}\CameraWebhook.exe"
Name: "{userdesktop}\Camera Webhook"; Filename: "{app}\CameraWebhook.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Создать ярлык на рабочем столе"; GroupDescription: "Дополнительные значки:"; Flags: unchecked

[Run]
Filename: "{app}\CameraWebhook.exe"; Description: "Запустить Camera Webhook"; Flags: nowait postinstall skipifsilent

