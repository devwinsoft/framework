set IDL=..\bin\Builder.exe
set SRC=Client.make
set TMPDIR=..\temp
set UNITY=..\applications\Unity\Assets

%IDL% -idl %SRC%				%TMPDIR%
xcopy      %TMPDIR%\*.cs		%UNITY%\Devarc\_Generated_Code\ /Y
move       %TMPDIR%\*.cs		.\Protocol.Client\_Generated_Code\

pause
