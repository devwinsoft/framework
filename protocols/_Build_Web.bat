set IDL=..\bin\Builder.exe
set TMPDIR=..\temp
set UNITY=..\applications\Unity\Assets

%IDL%  -php  Web.make          %TMPDIR%
xcopy        %TMPDIR%\*.cs     %UNITY%\Devarc\_Generated_Code\ /Y
move         %TMPDIR%\*.cs     .\Protocol.Web\_Generated_Code\
move         %TMPDIR%\*.php    .\Protocol.Web\_Generated_PHP\Protocol\

pause
