using System;
using System.Collections.Generic;
using System.Data;
#if UNITY_5 || UNITY_2017
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
using SqliteDataReader = System.Data.SQLite.SQLiteDataReader;
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
using SqliteTransaction = System.Data.SQLite.SQLiteTransaction;
using SqliteCommand = System.Data.SQLite.SQLiteCommand;
#endif

//  db file name = xxxx.db.bytes   <- .bytes 붙여야함..

namespace Devarc
{
    public class SQLite_Session
    {
        SqliteConnection mConn = null;
        SqliteTransaction mTrans = null;

        public bool Open(string _databasePath)
        {
            try
            {
                string conn = "URI=file:" + _databasePath;
                if (mConn == null)
                {
                    mConn = new SqliteConnection(conn);
                    mConn.Open();
                    mTrans = null;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool ExecuteNonQuery(string _command)
        {
            if (mTrans == null)
            {
                mTrans = mConn.BeginTransaction();
            }

            IDbCommand mCommand = mConn.CreateCommand();
            mCommand.Transaction = mTrans;

            if (mCommand == null)
            {
                Log.Error("SQLite is not opened.");
                return false;
            }

            try
            {
                mCommand.CommandText = _command;
                mCommand.ExecuteNonQuery();
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public SqliteDataReader ExecuteReader(string _query)
        {
            if (mTrans != null)
            {
                mTrans.Commit();
                mTrans = mConn.BeginTransaction();
            }

            SqliteCommand mCommand = new SqliteCommand(mConn);
            mCommand.Transaction = mTrans;
            mCommand.CommandText = _query;

            try
            {
                SqliteDataReader reader = mCommand.ExecuteReader();
                return reader;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public void Commit()
        {
            if (mTrans != null)
            {
                mTrans.Commit();
                mTrans = null;
            }
        }

        public void Close()
        {
            Commit();
            if (mConn != null)
            {
                mConn.Close();
                mConn = null;
            }
        }
    }

}
