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
        public bool IsClosed
        {
            get { return mReader == null || mReader.IsClosed; }
        }
        IDataReader mReader = null;
        Dictionary<string, int> mKeys = new Dictionary<string, int>();
        int mRowCount = 0;

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
                if (IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                if (mReader.Read())
                {
                    if (mRowCount == 0)
                    {
                        for (int i = 0; i < mReader.FieldCount; i++)
                        {
                            mKeys.Add(mReader.GetName(i), i);
                        }
                    }
                    mRowCount++;
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

        bool getStringValue(int _index, out string _value)
        {
            if (mReader.GetFieldType(_index) == typeof(string))
            {
                _value = mReader.GetString(_index);
                return true;
            }
            _value = string.Empty;
            return false;
        }

        public bool GetBoolean(string _name)
        {
            try
            {
                if (IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                int index = getKey(_name);
                string value;
                if (getStringValue(index, out value))
                {
                    if (string.IsNullOrEmpty(value))
                        return default(bool);
                    switch(value.ToLower())
                    {
                        case "0":
                        case "false":
                        case "f":
                        case "no":
                        case "n":
                            return false;
                        //case "true":
                        //case "t":
                        //case "yes":
                        //case "y":
                        default:
                            return true;
                    }
                }
                return mReader.GetBoolean(index);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        public short GetInt16(string _name)
        {
            try
            {
                if (IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                int index = getKey(_name);
                if (mReader.IsDBNull(index))
                {
                    return 0;
                }
                string value;
                if (getStringValue(index, out value))
                {
                    if (string.IsNullOrEmpty(value))
                        return default(short);
                    return short.Parse(value);
                }
                return mReader.GetInt16(index);
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
                if (IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                int index = getKey(_name);
                if (mReader.IsDBNull(index))
                {
                    return 0;
                }
                string value;
                if (getStringValue(index, out value))
                {
                    if (string.IsNullOrEmpty(value))
                        return default(int);
                    return int.Parse(value);
                }
                return mReader.GetInt32(index);
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
                if (IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                int index = getKey(_name);
                if (mReader.IsDBNull(index))
                {
                    return 0;
                }
                string value;
                if (getStringValue(index, out value))
                {
                    if (string.IsNullOrEmpty(value))
                        return default(long);
                    return long.Parse(value);
                }
                return mReader.GetInt64(index);
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
                if (IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                int index = getKey(_name);
                if (mReader.IsDBNull(index))
                {
                    return 0;
                }
                string value;
                if (getStringValue(index, out value))
                {
                    if (string.IsNullOrEmpty(value))
                        return default(uint);
                    return uint.Parse(value);
                }
                return (uint)mReader.GetDecimal(index);
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
                if (IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                int index = getKey(_name);
                if (mReader.IsDBNull(index))
                {
                    return 0;
                }
                string value;
                if (getStringValue(index, out value))
                {
                    if (string.IsNullOrEmpty(value))
                        return default(float);
                    return float.Parse(value);
                }
                return mReader.GetFloat(index);
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
                if (IsClosed)
                {
                    throw new Exception("[MySQL_Reader] Reader is closed.");
                }
                int index = getKey(_name);
                if (mReader.IsDBNull(index))
                {
                    return string.Empty;
                }
                string value;
                if (getStringValue(index, out value))
                {
                    return value;
                }
                return string.Empty;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                return string.Empty;
            }
        }
    }
}
