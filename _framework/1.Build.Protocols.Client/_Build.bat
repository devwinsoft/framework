set IDL=..\Bin\Builder.exe
set SRC=..\1.Build.Protocols.Client\Client.make
set TMPDIR=..\_TMP
set MODULE=..\Modules
set UNITY=..\..\applications\Unity\Assets

%IDL% -idl %SRC%				%TMPDIR%
xcopy      %TMPDIR%\*.cs		%UNITY%\Devarc\_GeneratedCode\ /Y
move       %TMPDIR%\*.cs		%MODULE%\Devarc.Net.Client\_GeneratedCode\

pause
