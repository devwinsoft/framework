set IDL=..\bin\Builder.exe
set SRC=Client.make
set TMPDIR=..\temp
set MODULE=..\modules
set UNITY=..\applications\Unity\Assets

%IDL% -idl %SRC%				%TMPDIR%
xcopy      %TMPDIR%\*.cs		%UNITY%\Devarc\_Generated_Code\ /Y
move       %TMPDIR%\*.cs		%MODULE%\Modules.Protocol.Client\_Generated_Code\

pause
