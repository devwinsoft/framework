set IDL=..\bin\Builder.exe
set SRC=Server.make
set TMPDIR=..\temp
set MODULE=..\modules
set UNITY=..\..\applications\Unity\Assets

%IDL%  -idl  %SRC%				%TMPDIR%
move         %TMPDIR%\*.cs		%MODULE%\Modules.Protocol.Server\_GeneratedCode\

pause