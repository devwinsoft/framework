using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.Serialization;

namespace SQLiteWrapper
{
    public class SQLiteException : DbException
    {
        #region Private Static Fields

        private static Dictionary<int, string> ErrorMessages = new Dictionary<int,string>();

        #endregion

        #region Private Fields

        private ResultCode errorCode;

        #endregion

        #region Static Constructor

        static SQLiteException()
        {
            PopulateErrorMessages();
        }

        #endregion

        #region Public Constructors

        public SQLiteException() 
            : base()
        { }

        public SQLiteException(string message) 
            : base(message) 
        { }

        private SQLiteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public SQLiteException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public SQLiteException(string message, int errorCode)
            : base(GetErrorMessage(message, errorCode)) 
        {
            this.errorCode = (ResultCode)errorCode;
        }

        #endregion

        #region Public Properties

        public new ResultCode ErrorCode
        {
            get { return errorCode; }
        }

        #endregion

        #region Private Static Methods

        private static void PopulateErrorMessages()
        {
            ErrorMessages.Add((int)ResultCode.SQLITE_OK, "Successful result");
            ErrorMessages.Add((int)ResultCode.SQLITE_ERROR, "SQL error or missing database");
            ErrorMessages.Add((int)ResultCode.SQLITE_INTERNAL, "Internal logic error in SQLite");
            ErrorMessages.Add((int)ResultCode.SQLITE_PERM, "Access permission denied");
            ErrorMessages.Add((int)ResultCode.SQLITE_ABORT, "Callback routine requested an abort");
            ErrorMessages.Add((int)ResultCode.SQLITE_BUSY, "The database file is locked");
            ErrorMessages.Add((int)ResultCode.SQLITE_LOCKED, "A table in the database is locked");
            ErrorMessages.Add((int)ResultCode.SQLITE_NOMEM, "A malloc() failed");
            ErrorMessages.Add((int)ResultCode.SQLITE_READONLY, "Attempt to write a readonly database");
            ErrorMessages.Add((int)ResultCode.SQLITE_INTERRUPT, "Operation terminated by sqlite3_interrupt()");
            ErrorMessages.Add((int)ResultCode.SQLITE_IOERR, "Some kind of disk I/O error occurred");
            ErrorMessages.Add((int)ResultCode.SQLITE_CORRUPT, "The database disk image is malformed");
            ErrorMessages.Add((int)ResultCode.SQLITE_NOTFOUND, "NOT USED. Table or record not found");
            ErrorMessages.Add((int)ResultCode.SQLITE_FULL, "Insertion failed because database is full");
            ErrorMessages.Add((int)ResultCode.SQLITE_CANTOPEN, "Unable to open the database file");
            ErrorMessages.Add((int)ResultCode.SQLITE_PROTOCOL, "NOT USED. Database lock protocol error");
            ErrorMessages.Add((int)ResultCode.SQLITE_EMPTY, "Database is empty");
            ErrorMessages.Add((int)ResultCode.SQLITE_SCHEMA, "The database schema changed");
            ErrorMessages.Add((int)ResultCode.SQLITE_TOOBIG, "String or BLOB exceeds size limit");
            ErrorMessages.Add((int)ResultCode.SQLITE_CONSTRAINT, "Abort due to constraint violation");
            ErrorMessages.Add((int)ResultCode.SQLITE_MISMATCH, "Data type mismatch");
            ErrorMessages.Add((int)ResultCode.SQLITE_MISUSE, "Library used incorrectly");
            ErrorMessages.Add((int)ResultCode.SQLITE_NOLFS, "Uses OS features not supported on host");
            ErrorMessages.Add((int)ResultCode.SQLITE_AUTH, "Authorization denied");
            ErrorMessages.Add((int)ResultCode.SQLITE_FORMAT, "Auxiliary database format error");
            ErrorMessages.Add((int)ResultCode.SQLITE_RANGE, "2nd parameter to sqlite3_bind out of range");
            ErrorMessages.Add((int)ResultCode.SQLITE_NOTADB, "File opened that is not a database file");
            ErrorMessages.Add((int)ResultCode.SQLITE_ROW, "sqlite3_step() has another row ready");
            ErrorMessages.Add((int)ResultCode.SQLITE_DONE, "sqlite3_step() has finished executing");
        }

        private static string GetErrorMessage(string message, int errorCode)
        {
            if (message == null) message = "";

            if (message.Length > 0)
                message = Environment.NewLine + message;

            string result = string.Empty;
            if (ErrorMessages.ContainsKey(errorCode))
                ErrorMessages.TryGetValue(errorCode, out result);
 
            return string.Format("{0},{1}", result, message);
        }

        #endregion
    }
}
