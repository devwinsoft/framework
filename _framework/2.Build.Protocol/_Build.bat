set IDL=Devarc.ProtocolBuilder.exe
set DLLDIR=..\_DLL
set MODULE=..\Modules
set UNITY=..\..\applications\Unity\Assets

%IDL% -net %DLLDIR%\Protocols.Client.dll
xcopy      %DLLDIR%\*.cs   %UNITY%\Devwinsoft\Devarc\_GeneratedCode\ /Y
move       %DLLDIR%\*.cs   %MODULE%\Devarc.Net.Client\_GeneratedCode\

%IDL% -net %DLLDIR%\Protocols.Server.dll
move       %DLLDIR%\*.cs   %MODULE%\Devarc.Net.Server\_GeneratedCode\

pause