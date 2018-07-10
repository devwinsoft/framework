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

        public bool Open(string _host, string _database)
        {
            string strconn = string.Format("Server={0};Database={1};Uid=ID;Pwd=PW;", _host, _database);
            try
            {
                mConnection = new MySqlConnection(strconn);
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
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public bool End_Transaction()
        {
            if (IsOpened == false)
            {
                return false;
            }

            try
            {
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }
    }
}
