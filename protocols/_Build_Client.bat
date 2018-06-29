set IDL=..\bin\Builder.exe
set SRC=Client.make
set TMPDIR=..\temp
set MODULE=..\modules
set UNITY=..\applications\Unity\Assets

%IDL% -idl %SRC%				%TMPDIR%
xcopy      %TMPDIR%\*.cs		%UNITY%\Devarc\_GeneratedCode\ /Y
move       %TMPDIR%\*.cs		%MODULE%\Modules.Client\_GeneratedCode\

pause
