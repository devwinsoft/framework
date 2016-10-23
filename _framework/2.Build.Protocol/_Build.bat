set IDL=Devarc.ProtocolBuilder.exe
set DLLDIR=..\_DLL
set TMPDIR=..\_TMP
set MODULE=..\Modules
set UNITY=..\..\applications\Unity\Assets

%IDL% -net %DLLDIR%\Protocols.Client.dll %TMPDIR%
xcopy      %TMPDIR%\*.cs   %UNITY%\Devwinsoft\Devarc\_GeneratedCode\ /Y
move       %TMPDIR%\*.cs   %MODULE%\Devarc.Net.Client\_GeneratedCode\

%IDL% -net %DLLDIR%\Protocols.Server.dll %TMPDIR%
move       %TMPDIR%\*.cs   %MODULE%\Devarc.Net.Server\_GeneratedCode\

pause