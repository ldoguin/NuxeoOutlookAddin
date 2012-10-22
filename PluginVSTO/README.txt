================== Plugin Mail - Compilation =======================
To compile those projects use MS Visual Studio 2010.

Missing references (on the References project) :

- DotCMIS.dll
- Newtonsoft.Json.dll
On Visual Studio 2010, select the menu Project>Add Reference. In the Browse Tab, select the missing DLL.

Rebuild the certificate :

- Right click on the project (OutlookAddIn1) and select Properties.
- On the Signing tab, change the password (the old password is swordgroup)


================== Plugin Mail - Installation ====================

==== Installation for Office 2003 - Prerequisites ====

- Microsoft .NET Framework 3.5
	DotNetFX35

- Microsoft Visual Studio Tools for Office System 3.0 Runtime
	VSTOR30.exe

- Microsoft Office Primary Interop assembly 2003
	OPIA2003.msi



==== Installation for Office 2007 - Prerequisites ====

- Microsoft .NET Framework 4 Client Profile (x86 and x64)
	dotNetFx40_Client_x86_x64.exe

- Microsoft Visual Studio 2010 Tools for Office Runtime (x86 and x64)
	vstor40x64.exe ou vstor40_x86.exe

- Office 2007 SP1 or later

- Hotfix KB976477


==== Installation for Office 2010 - Prerequisites ====

- Microsoft .NET Framework 4 Client Profile (x86 and x64)
	dotNetFx40_Client_x86_x64.exe

- Microsoft Visual Studio 2010 Tools for Office Runtime (x86 and x64)
	vstor40x64.exe ou vstor40_x86.exe
	
The setup of the pplugin is included in the followibng sub projects :
* OutlookAddIn1Installation for MS Outlook 2003
* InstallOutlookAddin for MS Outlook 2007 and 2010

================== Plugin Mail - Configuration ====================

Log file : C:\Users\{User name}\AppData\Roaming\SWORD\OutlookAddin\tmp\error.log
Configuration file : C:\Program Files (x86)\SWORD\