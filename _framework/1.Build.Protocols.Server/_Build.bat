set IDL=..\Bin\Builder.exe
set SRC=..\1.Build.Protocols.Server\Server.make
set TMPDIR=..\_TMP
set MODULE=..\Modules
set UNITY=..\..\applications\Unity\Assets

%IDL%  -idl  %SRC%				%TMPDIR%
move         %TMPDIR%\*.cs		%MODULE%\Devarc.Net.Server\_GeneratedCode\

pause