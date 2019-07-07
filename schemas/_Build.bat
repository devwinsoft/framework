set CMD=..\bin\Builder.exe
set TMPDIR=..\temp
set OUTDIR1=..\applications\Unity\Assets\Devarc\_Generated_Code
set OUTDIR2=.\Modules.Schema\_Generated_Code
set OUTDIR3=.\sql\MYSQL.InnoDB
set OUTDIR4=.\sql\MySQL.MyISAM
set OUTDIR5=.\sql\SQLite

%CMD%     -obj             LString.xlsx        %TMPDIR%
%CMD%     -obj             Sound.xlsx          %TMPDIR%
%CMD%     -obj             TestSchema.xlsx     %TMPDIR%
%CMD%     -obj             Examples.schema	   %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs    %OUTDIR1%\
move      %TMPDIR%\*.cs    %OUTDIR2%\
						  
%CMD%     -data            LString.xlsx      %TMPDIR%
%CMD%     -data            Sound.xlsx        %TMPDIR%
%CMD%     -data            TestSchema.xlsx   %TMPDIR%
xcopy /Y  %TMPDIR%\*.cs    %OUTDIR1%\
move      %TMPDIR%\*.cs    %OUTDIR2%\
						  
%CMD%     -mysql           TestSchema.xlsx   %TMPDIR%
move      %TMPDIR%\*.ddl   %OUTDIR3%\
move      %TMPDIR%\*.sql   %OUTDIR3%\

%CMD%     -sql             TestSchema.xlsx   %TMPDIR%
move      %TMPDIR%\*.ddl   %OUTDIR4%\
move      %TMPDIR%\*.sql   %OUTDIR4%\

%CMD%     -sqlite          TestSchema.xlsx   %TMPDIR%
move      %TMPDIR%\*.ddl   %OUTDIR5%\
move      %TMPDIR%\*.sql   %OUTDIR5%\

pause