set IDL=..\bin\Builder.exe
set SRC=Server.make
set TMPDIR=..\temp
set UNITY=..\..\applications\Unity\Assets

%IDL% -idl %SRC%           %TMPDIR%
move       %TMPDIR%\*.cs   .\Protocol.Server\_Generated_Code\

pause