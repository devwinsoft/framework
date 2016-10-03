set CMD=Devarc.TableBuilder.exe
set XMLDIR=..\_XML
set MODULE=..\Modules
set UNITY=..\..\applications\Unity\Assets

%CMD%     -obj            %XMLDIR%\ClientObject.xml
xcopy /Y  %XMLDIR%\*.cs   %UNITY%\_devarc\_GeneratedCode\
move      %XMLDIR%\*.cs   %MODULE%\Devarc.Object\_GeneratedCode\

%CMD%     -data           %XMLDIR%\ClientObject.xml
xcopy /Y  %XMLDIR%\*.cs   %UNITY%\_devarc\_GeneratedCode\
move      %XMLDIR%\*.cs   %MODULE%\Devarc.Table\_GeneratedCode\

%CMD%     -obj            %XMLDIR%\Message.xml
xcopy /Y  %XMLDIR%\*.cs   %UNITY%\_devarc\_GeneratedCode\
move      %XMLDIR%\*.cs   %MODULE%\Devarc.Object\_GeneratedCode\

%CMD%     -data           %XMLDIR%\Message.xml
xcopy /Y  %XMLDIR%\*.cs   %UNITY%\_devarc\_GeneratedCode\
move      %XMLDIR%\*.cs   %MODULE%\Devarc.Table\_GeneratedCode\

pause