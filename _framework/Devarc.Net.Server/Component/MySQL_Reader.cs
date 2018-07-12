using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Common;
using MySql.Data.MySqlClient;
using Devarc;

namespace Devarc
{
    public class MySQL_Reader : IBaseReader, IDisposable
    {
        IDataReader mReader = null;
        Dictionary<string, int> mKeys = new Dictionary<string, int>();

        private MySQL_Reader()
        {
        }

        public MySQL_Reader(IDataReader _reader)
        {
            mReader = _reader;
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (mReader != null)
            {
                mReader.Close();
                mReader = null;
            }
        }

        public bool Read()
        {

            try
            {
                if (mReader == null || mReader.IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                if (mReader.Read())
                {
                    for (int i = 0; i < mReader.FieldCount; i++)
                    {
                        mKeys.Add(mReader.GetName(i), i);
                    }
                    return true;
                }
                return false;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        int getKey(string _name)
        {
            int value;
            if (mKeys.TryGetValue(_name, out value))
                return value;
            throw new Exception(string.Format("[MySQL_Reader] Cannot find field name: {0}", _name));
        }

        public short GetInt16(string _name)
        {
            try
            {
                if (mReader == null || mReader.IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                return mReader.GetInt16(getKey(_name));
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public int GetInt32(string _name)
        {
            try
            {
                if (mReader == null || mReader.IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                return mReader.GetInt32(getKey(_name));
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public long GetInt64(string _name)
        {
            try
            {
                if (mReader == null || mReader.IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                return mReader.GetInt64(getKey(_name));
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public uint GetUInt32(string _name)
        {
            try
            {
                if (mReader == null || mReader.IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                return (uint)mReader.GetDecimal(getKey(_name));
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public float GetFloat(string _name)
        {
            try
            {
                if (mReader == null || mReader.IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                return mReader.GetFloat(getKey(_name));
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public string GetString(string _name)
        {
            try
            {
                if (mReader == null || mReader.IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                return mReader.GetString(getKey(_name));
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return string.Empty;
            }
        }
    }
}
