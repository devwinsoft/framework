set CMD=Devarc.TableBuilder.exe
set INDIR=..\..\applications\Unity\Assets\Devarc\ExcelData
set TMPDIR=..\_TMP
set OUTDIR1=..\..\applications\Unity\Assets\Devarc\_GeneratedCode
set OUTDIR2=..\Modules\Devarc.Object\_GeneratedCode

%CMD%     -obj           %INDIR%\TestSchema.xlsx  %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs  %OUTDIR1%\
move      %TMPDIR%\*.cs  %OUTDIR2%\

%CMD%     -data          %INDIR%\TestSchema.xlsx  %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs  %OUTDIR1%\
move      %TMPDIR%\*.cs  %OUTDIR2%\

pause