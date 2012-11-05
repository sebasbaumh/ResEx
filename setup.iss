[Setup]
OutputBaseFilename={#OutputFileName}
VersionInfoCompany=Dimitris Papadimitriou
AppName={#ProductName}
AppVerName={#ProductName} {#VersionShort}
ShowLanguageDialog=yes
OutputDir=
SourceDir=.\Executable\bin\{#Configuration}
DefaultDirName={pf}\ResEx
DisableProgramGroupPage=true
UsePreviousGroup=true
AppendDefaultGroupName=true
DefaultGroupName=ResEx
AppPublisher=Dimitris Papadimitriou
AppPublisherURL=http://resex.codeplex.com
AppSupportURL=http://resex.codeplex.com/Thread/List.aspx
AppUpdatesURL=http://resex.codeplex.com
UninstallDisplayIcon={app}\ResEx.exe
AppID={{4C16D937-263B-4BD7-9375-0F0858E48903}
VersionInfoVersion={#Version}
VersionInfoDescription=ResEx
InternalCompressLevel=max
ArchitecturesInstallIn64BitMode=x64
[Files]
DestDir: {app}; Source: ResEx.exe; Flags: replacesameversion
DestDir: {app}; Source: ResEx.exe.config; Flags: replacesameversion
DestDir: {app}; Source: ..\Release\ResEx.chm; Flags: replacesameversion
DestDir: {app}; Source: ResEx.Common.dll; Flags: replacesameversion
DestDir: {app}; Source: ResEx.Core.dll; Flags: replacesameversion
DestDir: {app}; Source: ResEx.StandardAdapters.dll; Flags: replacesameversion
DestDir: {app}; Source: ResEx.StandardPlugIns.dll; Flags: replacesameversion
DestDir: {app}; Source: ResEx.TranslationPlugin.dll; Flags: replacesameversion
DestDir: {app}; Source: ResEx.Win.dll; Flags: replacesameversion
DestDir: {app}; Source: Mvp.Xml.dll; Flags: replacesameversion
DestDir: {app}; Source: Microsoft.Practices.ObjectBuilder2.dll; Flags: replacesameversion
DestDir: {app}; Source: Microsoft.Practices.Unity.dll; Flags: replacesameversion
[Icons]
Name: {group}\ResEx; Filename: {app}\ResEx.exe; Comment: The composite, translation friendly .NET Resource editor; WorkingDir: {userdocs}; IconFilename: {app}\ResEx.exe; IconIndex: 1
Name: {group}\{cm:UninstallProgram, ResEx}; Filename: {uninstallexe}
Name: {group}\Web Site; Filename: {app}\WebSite.url
Name: {group}\Help; Filename: {app}\ResEx.chm
[Run]
Filename: {app}\ResEx.exe; Flags: postinstall nowait; Description: Run Installed Application
[Registry]
Root: HKCR; SubKey: .resx; ValueType: string; ValueData: .NET Resource File; Flags: uninsdeletekey; Tasks: " AssociateResX"
Root: HKCR; SubKey: .NET Resource File; ValueType: string; ValueData: .NET Resource File; Flags: uninsdeletekey; Tasks: " AssociateResX"
Root: HKCR; SubKey: .NET Resource File\Shell\Open\Command; ValueType: string; ValueData: """{app}\ResEx.exe"" ""%1"""; Flags: uninsdeletevalue; Tasks: " AssociateResX"
[Tasks]
Name: AssociateResX; Description: Associate .NET resource files (*.resx) with ResEx
[UninstallDelete]
Type: files; Name: {app}\WebSite.url
[InstallDelete]
Name: {app}\Extended.Common.dll; Type: files
Name: {app}\ResEx.exe.config; Type: files
[INI]
Filename: {app}\WebSite.url; Section: InternetShortcut; Key: URL; String: http://resex.codeplex.com
[Languages]
Name: English; MessagesFile: compiler:Default.isl
[CustomMessages]
NET35NotInstalled=.NET Framework 3.5 is required for this program. Please install .NET Framework 3.5 and try again. Would you like to go to download page now?
[Code]
function InitializeSetup(): Boolean;
var
	varErrorCode: Integer;
begin
	// check if .NET 3.5 is installed
	if (not RegKeyExists(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v3.5')) then
	begin
		case MsgBox(CustomMessage('NET35NotInstalled'), mbError, MB_YESNO) of
			IDYES: ShellExec('open', 'http://www.microsoft.com/downloads/details.aspx?FamilyId=333325FD-AE52-4E35-B531-508D977D32A6&displaylang=en','', '', SW_SHOW, ewNoWait, varErrorCode);
		end;
		Result := false;
	end else
		Result := true;
end;
