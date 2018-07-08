using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SQLiteWrapper
{
    internal static partial class SQLiteUnsafeNativeMethods
    {
        private const string Sqlite3DllName = "sqlite3.dll";

        [DllImport("kernel32")]
        public extern static int lstrlen(IntPtr str);

        public static string PtrToString(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;
            Encoding encoding = Encoding.UTF8;
            int length = GetPointerLenght(ptr);
            Byte[] bytes = new Byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            return encoding.GetString(bytes, 0, length);
        }

        public static int GetPointerLenght(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return 0;
            return SQLiteUnsafeNativeMethods.lstrlen(ptr);
        }

        #region Public Methods

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_aggregate_context(IntPtr sqlite3_context, int nBytes);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_auto_extension(sqlite3_auto_extension_delegate func);

        #region Online Backup API

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_backup_init(IntPtr pDest, IntPtr zDestName, IntPtr pSource, IntPtr zSourceName);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_backup_step(IntPtr sqlite3_backup, int nPage);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_backup_finish(IntPtr sqlite3_backup);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_backup_remaining(IntPtr sqlite3_backup);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_backup_pagecount(IntPtr sqlite3_backup);

        #endregion

        #region Binding Methods

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_blob(IntPtr stmt, int index, byte[] value, int nlen, IntPtr pvReserved);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_blob_reopen(IntPtr stmt, long value);
        
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_double(IntPtr stmt, int index, double value);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_int(IntPtr stmt, int index, int value);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_int64(IntPtr stmt, int index, long value);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_null(IntPtr stmt, int index);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_parameter_count(IntPtr stmt);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_parameter_index(IntPtr stmt, IntPtr zName);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_bind_parameter_name(IntPtr stmt, int index);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_text(IntPtr stmt, int index, IntPtr value, int nlen, IntPtr pvReserved);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_text16(IntPtr stmt, int index, IntPtr value, int nlen, IntPtr pvReserved);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_value(IntPtr stmt, int index, IntPtr sqlite3_value);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_bind_zeroblob(IntPtr stmt, int index, int nlen);

        #endregion

        #region Blob Methods

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_blob_bytes(IntPtr sqlite3_blob);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_blob_close(IntPtr sqlite3_blob);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_blob_open(IntPtr sqlite3, IntPtr zDb, IntPtr zTable, IntPtr zColumn, Int64 iRow, int flags, out IntPtr ppBlob);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_blob_read(IntPtr sqlite3_blob, byte[] Z, int N, int iOffset);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_blob_write(IntPtr sqlite3_blob, byte[] z, int n, int iOffset);

        #endregion

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_busy_handler(IntPtr sqlite3, sqlite3_busy_handler_delegate del, IntPtr value);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_busy_timeout(IntPtr sqlite3, int ms);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_changes(IntPtr sqlite3);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_clear_bindings(IntPtr sqlite3_stmt);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_close(IntPtr sqlite3);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_collation_needed(IntPtr sqlite3, 
                                                      IntPtr value, 
                                                      sqlite3_collation_needed_delegate del);
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_collation_needed16(IntPtr sqlite3, 
                                                  IntPtr value,
                                                  sqlite3_collation_needed16_delegate del);

        #region Result Values From A Query

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_blob(IntPtr sqlite3_stmt, int iCol);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_column_bytes(IntPtr sqlite3_stmt, int iCol);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_column_bytes16(IntPtr sqlite3_stmt, int iCol);

        [DllImport(Sqlite3DllName)]
        public static extern double sqlite3_column_double(IntPtr sqlite3_stmt, int iCol);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_column_int(IntPtr sqlite3_stmt, int iCol);

        [DllImport(Sqlite3DllName)]
        public static extern long sqlite3_column_int64(IntPtr sqlite3_stmt, int iCol);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_text(IntPtr sqlite3_stmt, int iCol);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_text16(IntPtr sqlite3_stmt, int iCol);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_column_type(IntPtr sqlite3_stmt, int iCol);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_value(IntPtr sqlite3_stmt, int iCol);

        #endregion

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_column_count(IntPtr sqlite3_stmt);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_database_name(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_database_name16(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_decltype(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_decltype16(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_name(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_name16(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_origin_name(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_origin_name16(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_table_name(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_column_table_name16(IntPtr sqlite3_stmt, int N);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_commit_hook(IntPtr sqlite3, sqlite3_commit_hook_delegate func, IntPtr arg);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_rollback_hook(IntPtr sqlite3, sqlite3_rollback_hook_delegate func, IntPtr arg);

        #region Determine If An SQL Statement Is Complete

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_complete(IntPtr sql);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_complete16(IntPtr sql);

        #endregion

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_config(int configOption, params IntPtr[] paramList);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_context_db_handle(IntPtr sqlite3_context);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_create_collation(IntPtr sqlite3, IntPtr zName, int eTextRep, IntPtr pvUser, 
            sqlite3_create_collation_delegate func);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_create_collation_v2(IntPtr sqlite3, IntPtr zName, int eTextRep, IntPtr pvUser, 
            sqlite3_create_collation_delegate func, 
            sqlite3_create_collation_delete_delegate delFunc);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_create_collation16(IntPtr sqlite3, IntPtr zName, int eTextRep, IntPtr pvUser, 
            sqlite3_create_collation_delegate func);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_create_function(IntPtr db, IntPtr strName, int nArgs, int nType, IntPtr pvUser,
            sqlite3_create_function_callback_delegate func,
            sqlite3_create_function_callback_delegate fstep, 
            sqlite3_create_function_final_callback_delegate ffinal);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_create_function16(IntPtr db, IntPtr zFunctionName, int nArg, int eTextRep, IntPtr pApp,
            sqlite3_create_function_callback_delegate func,
            sqlite3_create_function_callback_delegate fstep,
            sqlite3_create_function_final_callback_delegate ffinal);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_create_module(IntPtr db, IntPtr zName, IntPtr p, IntPtr pClientData);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_create_module_v2(IntPtr db, IntPtr zName, IntPtr p, IntPtr pClientData, 
            sqlite3_create_module_v2_delegate xDestroy);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_data_count(IntPtr pStmt);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_db_config(IntPtr sqlite3, int op, params object[] paramList);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_db_filename(IntPtr db, IntPtr zDbName);
        
        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_db_handle(IntPtr sqlite3_stmt);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_db_mutex(IntPtr sqlite3);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_db_release_memory(IntPtr sqlite3);
        
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_db_status(IntPtr sqlite3, int op, ref int pCur, ref int pHiwtr, int resetFlg);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_declare_vtab(IntPtr sqlite3, IntPtr zSQL);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_enable_load_extension(IntPtr db, int onoff);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_enable_shared_cache(int arg);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_errcode(IntPtr db);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_extended_errcode(IntPtr db);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_errmsg(IntPtr sqlite3);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_errmsg16(IntPtr sqlite3);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_exec(IntPtr db, IntPtr strSql, 
            sqlite3_exec_delegate pvCallback, IntPtr pvParam, out IntPtr errMsg);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_extended_result_codes(IntPtr sqlite3, int onoff);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_file_control(IntPtr sqlite3, IntPtr zDbName, int op, IntPtr arg);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_finalize(IntPtr pStmt);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_malloc(int nBytes);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_realloc(IntPtr buffer, int nBytes);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_free(IntPtr arg);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_get_table(IntPtr db, IntPtr zSql, ref IntPtr[] pazResult, ref int pnRow, ref int pnColumn, ref IntPtr pzErrmsg);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_free_table(ref IntPtr result);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_get_autocommit(IntPtr sqlite3);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_get_auxdata(IntPtr sqlite3_context, int N);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_set_auxdata(IntPtr sqlite3_context, int N, IntPtr aux, 
            sqlite3_set_auxdata_delegate xDelete);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_initialize();

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_shutdown();

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_os_init();

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_os_end();

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_interrupt(IntPtr sqlite3);

        [DllImport(Sqlite3DllName)]
        public static extern long sqlite3_last_insert_rowid(IntPtr sqlite3);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_libversion();

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_sourceid();

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_libversion_number();

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_limit(IntPtr sqlite3, int id, int newVal);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_load_extension(IntPtr db, IntPtr zFile, IntPtr zProc, ref IntPtr pzErrMsg);

        [DllImport(Sqlite3DllName)]
        public static extern long sqlite3_memory_used();

        [DllImport(Sqlite3DllName)]
        public static extern long sqlite3_memory_highwater(int resetFlag);

        /*TODO
         * 
        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_mprintf(IntPtr ,...);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_vmprintf(IntPtr str, va_list);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_snprintf(int,char*,const char*, ...);
         */

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_mutex_alloc(MutexTypes mutexType);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_mutex_free(IntPtr sqlite3_mutex);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_mutex_enter(IntPtr sqlite3_mutex);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_mutex_try(IntPtr sqlite3_mutex);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_mutex_leave(IntPtr sqlite3_mutex);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_mutex_held(IntPtr sqlite3_mutex);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_mutex_notheld(IntPtr sqlite3_mutex);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_next_stmt(IntPtr pDb, IntPtr pStmt);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_open(IntPtr filename, out IntPtr ppDb);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_open16(IntPtr filename, out IntPtr ppDb);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_open_v2(IntPtr filename, out IntPtr ppDb, int flags, IntPtr zVfs);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_overload_function(IntPtr sqlite3, IntPtr zFuncName, int nArg);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_prepare(IntPtr ppDb, IntPtr zSql, int nByte, out IntPtr ppStmt, out IntPtr pzTail);
 
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_prepare_v2(IntPtr ppDb, IntPtr zSql, int nByte, out IntPtr ppStmt, out IntPtr pzTail);
  
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_prepare16(IntPtr ppDb, IntPtr zSql, int nByte, out IntPtr ppStmt, out IntPtr pzTail);
 
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_prepare16_v2(IntPtr ppDb, IntPtr zSql, int nByte, out IntPtr ppStmt, out IntPtr pzTail);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_trace(IntPtr sqlite3, sqlite3_trace_delegate func, IntPtr arg);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_profile(IntPtr sqlite3, sqlite3_profile_delegate func, IntPtr arg);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_progress_handler(IntPtr sqlite3, int nOps, sqlite3_dxProgress_delegate func, IntPtr pProgressArg);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_randomness(int N, IntPtr P);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_release_memory(int nBytes);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_reset(IntPtr pStmt);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_reset_auto_extension();

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_blob(IntPtr sqlite3_context, IntPtr content, int nBytes, sqlite3_result_blob_delegate func);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_double(IntPtr sqlite3_context, double arg);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_error(IntPtr sqlite3_context, IntPtr msg, int nBytes);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_error16(IntPtr sqlite3_context, IntPtr msg, int nBytes);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_error_toobig(IntPtr sqlite3_context);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_error_nomem(IntPtr sqlite3_context);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_error_code(IntPtr sqlite3_context, int errCode);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_int(IntPtr sqlite3_context, int value);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_int64(IntPtr sqlite3_context, Int64 value);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_null(IntPtr sqlite3_context);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_text(IntPtr sqlite3_context, IntPtr msg, int n, 
            sqlite3_result_text_delegate func);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_text16(IntPtr sqlite3_context, IntPtr msg, int n, 
            sqlite3_result_text16_delegate func);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_text16le(IntPtr sqlite3_context, IntPtr msg, int n, 
            sqlite3_result_text16le_delegate func);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_text16be(IntPtr sqlite3_context, IntPtr msg, int n, 
            sqlite3_result_text16be_delegate func);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_value(IntPtr sqlite3_context, IntPtr sqlite3_value);

        [DllImport(Sqlite3DllName)]
        public static extern void sqlite3_result_zeroblob(IntPtr sqlite3_context, int n);
 
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_set_authorizer(IntPtr sqlite3, 
            sqlite3_set_authorizer_delegate func, IntPtr pUserData);
 
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_sleep(int nMilliseconds);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_sql(IntPtr pStmt);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_status(int op, ref int pCurrent, ref int pHighwater, int resetFlag);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_step(IntPtr stmt);
        
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_stmt_busy(IntPtr sqlite3_stmt);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_stmt_readonly(IntPtr pStmt);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_stmt_status(IntPtr sqlite3_stmt, int op, int resetFlg);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_strnicmp(IntPtr arg1, IntPtr arg2, int n);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_table_column_metadata(IntPtr db,
                              IntPtr zDbName, IntPtr zTableName, IntPtr zColumnName, out IntPtr pzDataType, 
                              out IntPtr pzCollSeq, out int pNotNull, out int pPrimaryKey, out int pAutoinc);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_test_control(int op, params IntPtr[] arg);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_threadsafe();

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_total_changes(IntPtr sqlite3);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_unlock_notify(IntPtr pBlocked, sqlite3_unlock_notify_delegate func, IntPtr pNotifyArg);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_update_hook(IntPtr sqlite3, sqlite3_update_hook_delegate func,IntPtr arg);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_uri_parameter(IntPtr zFilename, IntPtr zParam);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_uri_boolean(IntPtr zFile, IntPtr zParam, int bDefault);

        [DllImport(Sqlite3DllName)]
        public static extern long sqlite3_uri_int64(IntPtr zFilename, IntPtr zParam, long D);
        
        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_user_data(IntPtr sqlite3_context);

        #region Obtaining SQL Function Parameter Values

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_value_blob(IntPtr sqlite3_value);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_value_bytes(IntPtr sqlite3_value);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_value_bytes16(IntPtr sqlite3_value);

        [DllImport(Sqlite3DllName)]
        public static extern double sqlite3_value_double(IntPtr sqlite3_value);
        
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_value_int(IntPtr sqlite3_value);
        
        [DllImport(Sqlite3DllName)]
        public static extern Int64 sqlite3_value_int64(IntPtr sqlite3_value);
        
        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_value_text(IntPtr sqlite3_value);
        
        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_value_text16(IntPtr sqlite3_value);
        
        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_value_text16le(IntPtr sqlite3_value);
        
        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_value_text16be(IntPtr sqlite3_value);
        
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_value_type(IntPtr sqlite3_value);
        
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_value_numeric_type(IntPtr sqlite3_value);

        #endregion

        #region Virtual File System Objects

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_vfs_find(IntPtr zVfsName);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_vfs_register(IntPtr sqlite3_vfs, int makeDflt);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_vfs_unregister(IntPtr sqlite3_vfs);
 
        #endregion

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_vtab_config(IntPtr sqlite3, int op, params IntPtr[] paramList);
        
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_vtab_on_conflict(IntPtr sqlite3);
        
        //3.6.23

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_compileoption_used(IntPtr zOptName);

        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_compileoption_get(int N);
        
        //3.7.2
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_wal_autocheckpoint(IntPtr sqlite3, int N);

        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_wal_checkpoint(IntPtr sqlite3, IntPtr zDb);

		[DllImport(Sqlite3DllName)]
		public static extern int sqlite3_wal_checkpoint_v2(IntPtr db, IntPtr zDb, int eMode, ref int pnLog, ref int pnCkpt);
        
        [DllImport(Sqlite3DllName)]
        public static extern IntPtr sqlite3_wal_hook(IntPtr sqlite3, sqlite3_wal_hook_delegate func, IntPtr pArg);

        //3.7.3
        [DllImport(Sqlite3DllName)]
        public static extern int sqlite3_create_function_v2(IntPtr db, IntPtr zFunctionName, int nArg, int eTextRep, IntPtr pApp,
            sqlite3_create_function_callback_delegate func,
            sqlite3_create_function_callback_delegate fstep,
            sqlite3_create_function_final_callback_delegate ffinal,
            sqlite3_create_function_destroy_delegate fdestroy);

        [DllImport(Sqlite3DllName)]
        public static extern long sqlite3_soft_heap_limit64(long nBytes);

        #endregion        
    }
}
