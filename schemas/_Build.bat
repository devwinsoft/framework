set CMD=..\bin\Builder.exe
set TMPDIR=..\temp
set OUTDIR1=..\applications\Unity\Assets\Devarc\_GeneratedCode
set OUTDIR2=..\modules\Modules.Schema\_GeneratedCode

%CMD%     -obj           LString.xlsx      %TMPDIR%
%CMD%     -obj           TestSchema.xlsx   %TMPDIR%
%CMD%     -obj           Examples.schema   %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs  %OUTDIR1%\
move      %TMPDIR%\*.cs  %OUTDIR2%\

%CMD%     -data          LString.xlsx      %TMPDIR%
%CMD%     -data          TestSchema.xlsx   %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs  %OUTDIR1%\
move      %TMPDIR%\*.cs  %OUTDIR2%\

pause