set CMD=Devarc.TableBuilder.exe
set INDIR=..\..\applications\Unity\Assets\Devwinsoft\Devarc\_test\Editor
set OUTDIR1=..\..\applications\Unity\Assets
set OUTDIR2=..\Modules

%CMD%     -obj           %INDIR%\TestSchema.xml
xcopy /Y  %INDIR%\*.cs   %OUTDIR1%\Devwinsoft\Devarc\_GeneratedCode\
move      %INDIR%\*.cs   %OUTDIR2%\Devarc.Object\_GeneratedCode\

%CMD%     -data          %INDIR%\TestSchema.xml
xcopy /Y  %INDIR%\*.cs   %OUTDIR1%\Devwinsoft\Devarc\_GeneratedCode\
move      %INDIR%\*.cs   %OUTDIR2%\Devarc.Table\_GeneratedCode\

pause