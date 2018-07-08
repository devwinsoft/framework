using System;

namespace SQLiteWrapper
{
    internal static partial class SQLiteUnsafeNativeMethods
    {
        public delegate void sqlite3_auto_extension_delegate();
        public delegate int sqlite3_busy_handler_delegate(IntPtr value, int noOfInvokes);
        public delegate void sqlite3_collation_needed_delegate(IntPtr value, IntPtr sqlite3, int eTextRep, IntPtr collationName);
        public delegate void sqlite3_collation_needed16_delegate(IntPtr value, IntPtr sqlite3, int eTextRep, IntPtr collationName);

        public delegate int sqlite3_commit_hook_delegate(IntPtr arg);
        public delegate void sqlite3_rollback_hook_delegate(IntPtr arg);

        public delegate int sqlite3_create_collation_delegate(IntPtr arg, int len1, IntPtr pv1, int len2, IntPtr pv2);
        public delegate void sqlite3_create_collation_delete_delegate(IntPtr arg);

        public delegate void sqlite3_create_function_callback_delegate(IntPtr context, int nArgs, ref IntPtr argsptr);
        public delegate void sqlite3_create_function_final_callback_delegate(IntPtr context);

        public delegate void sqlite3_create_module_v2_delegate(IntPtr arg);
        public delegate int sqlite3_exec_delegate(IntPtr arg, int nColumns, IntPtr[] value, IntPtr[] colNames);

        public delegate void sqlite3_set_auxdata_delegate(IntPtr arg);

        public delegate void sqlite3_trace_delegate(IntPtr arg, IntPtr str);
        public delegate void sqlite3_profile_delegate(IntPtr arg, IntPtr str, UInt64 time);

        public delegate int sqlite3_dxProgress_delegate(IntPtr pProgressArg);

        public delegate void sqlite3_result_blob_delegate(IntPtr arg);
        public delegate void sqlite3_result_text_delegate(IntPtr arg);
        public delegate void sqlite3_result_text16_delegate(IntPtr arg);
        public delegate void sqlite3_result_text16le_delegate(IntPtr arg);
        public delegate void sqlite3_result_text16be_delegate(IntPtr arg);

        public delegate void sqlite3_set_authorizer_delegate(IntPtr arg, AuthorizerActionCode code, IntPtr s1, IntPtr s2, IntPtr s3, IntPtr s4);
        public delegate void sqlite3_unlock_notify_delegate(IntPtr[] apArg, int nArg);

        public delegate void sqlite3_update_hook_delegate(IntPtr arg, int authCode, IntPtr dbName, IntPtr tableName, Int64 rowId);

        public delegate void sqlite3_wal_hook_delegate(IntPtr pArg, IntPtr sqlite3, IntPtr dbName, int nPages);

        public delegate void sqlite3_create_function_destroy_delegate(IntPtr arg);
    }
}
