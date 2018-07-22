set IDL=..\bin\Builder.exe
set TMPDIR=..\temp
set MODULE=..\modules
set UNITY=..\applications\Unity\Assets

%IDL%  -php  WWW.make          %TMPDIR%
xcopy        %TMPDIR%\*.cs     %UNITY%\Devarc\_Generated_Code\ /Y
move         %TMPDIR%\*.cs     %MODULE%\Modules.Protocol.Web\_Generated_Code\
move         %TMPDIR%\*.php    %MODULE%\Modules.Protocol.Web\_Generated_PHP\Protocol\

pause
