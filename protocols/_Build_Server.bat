set IDL=..\bin\Builder.exe
set TMPDIR=..\temp
set MODULE=..\modules
set UNITY=..\..\applications\Unity\Assets

%IDL%  -idl  Server.make		%TMPDIR%
move         %TMPDIR%\*.cs		%MODULE%\Modules.Protocol.Server\_GeneratedCode\

pause