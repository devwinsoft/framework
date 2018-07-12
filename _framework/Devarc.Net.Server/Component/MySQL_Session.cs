using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Common;
using MySql.Data.MySqlClient;

namespace Devarc
{
    public class MySQL_Session : IDisposable
    {
        MySqlConnection mConnection = null;
        MySqlTransaction mTransaction = null;
        
        public ConnectionState State
        {
            get { return mConnection != null ? mConnection.State : ConnectionState.Closed; }
        }

        public bool IsOpened
        {
            get
            {
                if (mConnection == null)
                    return false;
                switch(mConnection.State)
                {
                    case ConnectionState.Open:
                    case ConnectionState.Executing:
                    case ConnectionState.Fetching:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public void Dispose()
        {
            Close();
        }

        public bool Open(string _host, string _database, string _user, string _passwd)
        {
            string connStr = string.Format("Server={0};Database={1};Uid={2};Pwd={3};", _host, _database, _user, _passwd);
            try
            {
                mConnection = new MySqlConnection(connStr);
                mConnection.Open();
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                if (mConnection != null)
                {
                    mConnection.Close();
                    mConnection = null;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool Begin_Transaction()
        {
            if (IsOpened == false)
            {
                return false;
            }

            try
            {
                mTransaction = mConnection.BeginTransaction();
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool Commit()
        {
            if (IsOpened == false)
            {
                return false;
            }

            if (mTransaction == null)
            {
                return false;
            }

            try
            {
                mTransaction.Commit();
                mTransaction = null;
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool RollBack()
        {
            if (IsOpened == false)
            {
                return false;
            }

            if (mTransaction == null)
            {
                return false;
            }

            try
            {
                mTransaction.Rollback();
                mTransaction = null;
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool Execute_NonQuery(string _query)
        {
            try
            {
                IDbCommand command = mConnection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = _query;
                command.ExecuteNonQuery();
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public MySQL_Reader Execute_Reader(string _query)
        {
            try
            {
                IDbCommand command = mConnection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = _query;
                IDataReader reader = command.ExecuteReader();
                MySQL_Reader tmpReader = new MySQL_Reader(reader);
                return tmpReader;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }
    }
}
