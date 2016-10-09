set CMD=Devarc.TableBuilder.exe
set XMLDIR=..\_XML
set MODULE=..\Modules
set UNITY=..\..\applications\Unity\Assets

%CMD%     -obj            %XMLDIR%\TestSchema.xml
xcopy /Y  %XMLDIR%\*.cs   %UNITY%\Devwinsoft\Devarc\_GeneratedCode\
move      %XMLDIR%\*.cs   %MODULE%\Devarc.Object\_GeneratedCode\

%CMD%     -data           %XMLDIR%\TestSchema.xml
xcopy /Y  %XMLDIR%\*.cs   %UNITY%\Devwinsoft\Devarc\_GeneratedCode\
move      %XMLDIR%\*.cs   %MODULE%\Devarc.Table\_GeneratedCode\

pause