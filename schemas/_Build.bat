set CMD=..\bin\Builder.exe
set TMPDIR=..\temp
set OUTDIR1=..\applications\Unity\Assets\Devarc\_Generated_Code
set OUTDIR2=..\modules\Modules.Schema\_Generated_Code
set OUTDIR3=..\modules\Modules.Schema\_Generated_MYSQL
set OUTDIR4=..\modules\Modules.Schema\_Generated_SQL
set OUTDIR5=..\applications\Unity\Assets\Devarc\_Generated_SQLite
set OUTDIR6=..\modules\Modules.Schema\_Generated_SQLite

%CMD%     -obj             LString.xlsx      %TMPDIR%
%CMD%     -obj             TestSchema.xlsx   %TMPDIR%
%CMD%     -obj             Examples.schema   %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs    %OUTDIR1%\
move      %TMPDIR%\*.cs    %OUTDIR2%\
						  
%CMD%     -data            LString.xlsx      %TMPDIR%
%CMD%     -data            TestSchema.xlsx   %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs    %OUTDIR1%\
move      %TMPDIR%\*.cs    %OUTDIR2%\
						  
%CMD%     -mysql           TestSchema.xlsx   %TMPDIR%
move      %TMPDIR%\*.sql   %OUTDIR3%\

%CMD%     -sql             TestSchema.xlsx   %TMPDIR%
move      %TMPDIR%\*.sql   %OUTDIR4%\

%CMD%     -sqlite          TestSchema.xlsx   %TMPDIR%
xcopy /Y  %TMPDIR%\*.sql   %OUTDIR5%\
move      %TMPDIR%\*.sql   %OUTDIR6%\

pause