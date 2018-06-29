set CMD=..\Bin\Builder.exe
set INDIR=..\..\schemas
set TMPDIR=..\_TMP
set OUTDIR1=..\..\applications\Unity\Assets\Devarc\_GeneratedCode
set OUTDIR2=..\..\modules\Modules.Data\_GeneratedCode

%CMD%     -obj           %INDIR%\TestSchema.xlsx  %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs  %OUTDIR1%\
move      %TMPDIR%\*.cs  %OUTDIR2%\

%CMD%     -data          %INDIR%\TestSchema.xlsx  %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs  %OUTDIR1%\
move      %TMPDIR%\*.cs  %OUTDIR2%\

pause