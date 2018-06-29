set CMD=..\bin\Builder.exe
set TMPDIR=..\temp
set OUTDIR1=..\applications\Unity\Assets\Devarc\_GeneratedCode
set OUTDIR2=..\modules\Modules.Schema\_GeneratedCode

%CMD%     -obj           TestSchema.xlsx  %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs  %OUTDIR1%\
move      %TMPDIR%\*.cs  %OUTDIR2%\

%CMD%     -data          TestSchema.xlsx  %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs  %OUTDIR1%\
move      %TMPDIR%\*.cs  %OUTDIR2%\

pause